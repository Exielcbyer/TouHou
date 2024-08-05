using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;

public class BOSS : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BehaviorTree behaviorTree;
    public GameObject hitFXPrefab;

    Vector2 size = new Vector2(1, 1.4f);
    Collider2D[] colliders = new Collider2D[100];
    private Transform target => GameObject.Find("Player").transform;
    public BossDetails_SO bossDetails;

    public float AccumulatedDamage;
    public bool isDead;
    public bool isGravity;
    public bool DownDeriving;
    private float currentHorizontalSpeed, currentVerticalSpeed;

    [Header("碰撞")]
    [SerializeField] private Bounds characterBounds;// 碰撞体
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int detectorCount = 3;// 检测器数量
    [SerializeField] private float detectionRayLength = 0.1f;// 检测器长度
    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;// 防止侧面检测器撞击地面

    //上下左右四个方向的碰撞检测器
    private RayRange raysDown;
    private bool colDown;
    private bool dashing;

    [Header("重力")]
    [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 80f;// 最小下落速度
    [SerializeField] private float maxFallSpeed = 120f;// 最大下落速度
    private float fallAcceleration = 80f;
    private float gravityDiscounts = 0.05f;// 重力折扣

    [SerializeField] private float jumpApexThreshold = 10f;// 跳跃顶点阈值
    private float apexPoint;// 在跳跃顶点变为1


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener("BehaviorTreeEnableEvent", OnBehaviorTreeEnableEvent);
        EventHandler.AddEventListener("ConquerOver", OnConquerOverEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener("BehaviorTreeEnableEvent", OnBehaviorTreeEnableEvent);
        EventHandler.RemoveEventListener("ConquerOver", OnConquerOverEvent);
    }

    private void Start()
    {
        isGravity = true;
        UIManager.Instance.SetBossHealth(bossDetails.maxHeath);
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Pause) return;
        if (isDead) return;

        if (bossDetails.heath <= 0 && !isDead) 
        {
            bossDetails.heath = 0;
            isDead = true;
            behaviorTree.DisableBehavior();
            EventHandler.TriggerEvent("BossDeadEvent");
            // 播放结束演出
            EventHandler.TriggerEvent("FilmShadeEvent", 100f);
            EventHandler.TriggerEvent<int>("TimeLinePlayEvent", 2);
            return;
        }

        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            //if (coll.gameObject.TryGetComponent(out BaseBarrage barrage))
            //{
            //    if (barrage.isFrozen)
            //    {
            //        GetHit(barrage.damage);
            //        barrage.CollEnemy();
            //    }
            //}
            if (coll.gameObject.TryGetComponent(out AttackArea attackArea))
            {
                if (!attackArea.attackOver)
                {
                    GetHit(attackArea.damage);
                    attackArea.attackOver = true;
                }
            }
            if (coll.gameObject.TryGetComponent(out BaseRanged ranged))
            {
                GetHit(ranged.damage);
                ranged.CollEnemy();
            }
        }

        RunCollisionChecks();
        CalculateGravity();
        SpeedCorrection();
        BarrageTest();
    }

    #region Collisions
    private void RunCollisionChecks()
    {
        // 生成射线范围. 
        CalculateRayRanged();

        // 地面检测
        var groundedCheck = RunDetection(raysDown);
        if (!colDown && groundedCheck)
        {
            DownDeriving = false;
        }

        colDown = groundedCheck;

        // 射线检测闭包，如果检测到物体则返回true
        bool RunDetection(RayRange range)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, groundLayer));
        }
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position, characterBounds.size);

        raysDown = new RayRange(b.min.x + rayBuffer, b.min.y, b.max.x - rayBuffer, b.min.y, Vector2.down);
    }


    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < detectorCount; i++)
        {
            var t = (float)i / (detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + characterBounds.center, characterBounds.size);

        // Rays
        if (!Application.isPlaying)
        {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { raysDown })
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;
    }

    #endregion

    #region Gravity
    private void CalculateGravity()
    {
        if (isGravity)
        {
            CalculateJumpApex();
            if (colDown)
            {
                if (currentVerticalSpeed < 0)
                    currentVerticalSpeed = 0;
            }
            else
            {
                // 攻击期间受到的重力效果打折
                var fallAcceleration = DownDeriving && currentVerticalSpeed < 0 ? this.fallAcceleration * gravityDiscounts : this.fallAcceleration;

                // 下落
                currentVerticalSpeed -= fallAcceleration * Time.deltaTime;

                // Clamp
                if (currentVerticalSpeed < fallClamp)
                    currentVerticalSpeed = fallClamp;
            }
        }
    }

    private void CalculateJumpApex()
    {
        if (!colDown)
        {
            // 如果离跳跃顶点更近，下落加速度就更强
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(currentVerticalSpeed));
            fallAcceleration = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }
    }
    #endregion

    private void GetHit(float damage)
    {
        AccumulatedDamage += damage;
        bossDetails.heath -= damage;
        EventHandler.TriggerEvent<float, float>("UpdateBossHealth", bossDetails.heath, bossDetails.maxHeath);
        spriteRenderer.DOColor(Color.red, 1.2f).SetEase(Ease.Flash, 8, 1).OnComplete(()=> { spriteRenderer.color = Color.white; });
        transform.DOShakePosition(0.1f, 0.2f, 5, 90);
        var hitFX = ObjectPool.Instance.GetObject(hitFXPrefab);
        hitFX.transform.position = transform.position;
    }

    private void SpeedCorrection()
    {
        if (!colDown || dashing)  
        {
            if (transform.rotation == Quaternion.Euler(0f, 0f, 0f))
            {
                transform.position += new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
            }
            else
            {
                transform.position += new Vector3(-currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
            }
        }
    }

    public void SetSpeed(float horizontalSpeed, float verticalSpeed)
    {
        if (horizontalSpeed == 0 && verticalSpeed == 0)
            dashing = false;
        else
            dashing = true;
        var direction = (target.position - transform.position).normalized;
        currentHorizontalSpeed = direction.x * horizontalSpeed;
        currentVerticalSpeed = verticalSpeed;
    }

    private void OnBehaviorTreeEnableEvent()
    {
        behaviorTree.EnableBehavior();
    }

    private void OnConquerOverEvent()
    {
        transform.position = new Vector3(0, -8.86f, 0);
    }

    // 测试弹幕生成
    private void BarrageTest()
    {
        // 小型圆形弹幕
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var direction = (target.position - transform.position).normalized;
            CircularBarrageFactory.Instance.CreatBarrage(transform.position, direction);
        }
        // 大型圆形弹幕
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var direction = (target.position - transform.position).normalized;
            var angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
            BigCircularBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle, transform);
        }
        // 菱形弹幕
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var direction = (target.position - transform.position).normalized;
            DiamondBarrageFactory.Instance.CreatBarrage(transform.position, direction, 0, target);
        }
        // 方形弹幕
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var direction = transform.localScale;
            SquareBarrageFactory.Instance.CreatBarrage(transform.position, direction);
        }
        // 柱状弹幕
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Vector3 viewPoint = Input.mousePosition;
            viewPoint.z -= Camera.main.transform.position.z;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(viewPoint);
            var direction = transform.localScale;
            ColumnarBarrageFactory.Instance.CreatBarrage(mouseWorldPos, direction);
        }
        // 直线弹幕
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            var direction = transform.localScale;
            StraightLineFactory.Instance.CreatBarrage(transform.position, direction, 0f, target);
        }
        // 光柱弹幕
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            var direction = transform.localScale;
            LightRodBarrageFactory.Instance.CreatBarrage(transform.position, direction);
        }
        // 射线弹幕
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            var direction = transform.localScale;
            LaserBarrageFactory.Instance.CreatBarrage(transform.position, direction);
        }        
        // 光炮弹幕
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            var direction = transform.localScale;
            CannonFactory.Instance.CreatBarrage(transform.position, direction, 0f);
        }
    }
}
