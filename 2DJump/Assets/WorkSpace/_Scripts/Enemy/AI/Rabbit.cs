using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rabbit : Enemy
{
    private float currentHorizontalSpeed = 4f;
    private float currentVerticalSpeed;

    private Vector3 leftTargetPos;
    private Vector3 rightTargetPos;

    [Header("碰撞")]
    [SerializeField] private Bounds characterBounds;// 碰撞体
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int detectorCount = 3;// 检测器数量
    [SerializeField] private float detectionRayLength = 0.1f;// 检测器长度
    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;// 防止侧面检测器撞击地面

    //上下左右四个方向的碰撞检测器
    private RayRange raysDown;
    private bool colDown;

    [Header("重力")]
    [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 80f;// 最小下落速度
    [SerializeField] private float maxFallSpeed = 120f;// 最大下落速度
    private float fallAcceleration = 80f;
    private float gravityDiscounts = 0.1f;// 重力折扣
    private float maxSpeedDiscounts = 0.5f;// 最大速度折扣

    [SerializeField] private float jumpApexThreshold = 10f;// 跳跃顶点阈值
    private float apexPoint;// 在跳跃顶点变为1

    public bool DownDeriving;

    [Header("GameObject")]
    public GameObject attackParticlesPrefab;

    protected override void Start()
    {
        base.Start();
        distance = new Vector2(1.6f, 0.5f);
    }

    protected override void Update()
    {
        base.Update();
        RunCollisionChecks();
        CalculateGravity();
        SpeedCorrection();
        anim.SetBool("DownDeriving", DownDeriving);
        anim.SetFloat("currentHorizontalSpeed", currentHorizontalSpeed);
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
        if (GameManager.Instance.gameState == GameState.Pause) return;
        CalculateJumpApex();
        if (colDown)
        {
            if (currentVerticalSpeed < 0)
                currentVerticalSpeed = 0;
        }
        else
        {
            // 攻击期间受到的重力效果打折
            var fallAcceleration = attacking && DownDeriving && currentVerticalSpeed < 0 ? this.fallAcceleration * gravityDiscounts : this.fallAcceleration;

            // 下落
            currentVerticalSpeed -= fallAcceleration * Time.deltaTime;

            // Clamp，攻击期间最大速度减小
            if (!attacking || DownDeriving)
            {
                if (currentVerticalSpeed < fallClamp)
                    currentVerticalSpeed = fallClamp;
            }
            else
            {
                if (currentVerticalSpeed < fallClamp * maxSpeedDiscounts)
                    currentVerticalSpeed = fallClamp * maxSpeedDiscounts;
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

    public override void Move()
    {
        if (!attacking)
        {
            if (transform.position.x < targetPoint.position.x)
            {
                leftTargetPos = new Vector3(targetPoint.position.x - 1.5f, transform.position.y, 0);
                transform.position = Vector2.MoveTowards(transform.position, leftTargetPos, currentHorizontalSpeed * Time.deltaTime);
                if (Mathf.Abs(transform.position.x - leftTargetPos.x) < 0.1f) 
                    currentHorizontalSpeed = 0;
                else
                    currentHorizontalSpeed = speed;
            }
            else
            {
                rightTargetPos = new Vector3(targetPoint.position.x + 1.5f, transform.position.y, 0);
                transform.position = Vector2.MoveTowards(transform.position, rightTargetPos, currentHorizontalSpeed * Time.deltaTime);
                if (Mathf.Abs(transform.position.x - rightTargetPos.x) < 0.1f)
                    currentHorizontalSpeed = 0;
                else
                    currentHorizontalSpeed = speed;
            }

            base.Move();
        }
    }

    public override void AttackAtion()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange) //两点间的距离，可判断x，y轴
        {
            if (Time.time > nextAttack && !attacking)
            {
                attacking = true;
                anim.SetTrigger("Attack");
                nextAttack = Time.time + attackRace;
                StopCoroutine("StartAttack");
                StartCoroutine("StartAttack");
                attackRandomCount = Random.Range(0, 10);
            }
        }
    }

    private IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(0.18f);
        currentHorizontalSpeed = 20f;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0f;
        yield return new WaitForSeconds(1.29f);
        currentHorizontalSpeed = 20f;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0f;
        yield return new WaitForSeconds(1.35f);
        currentHorizontalSpeed = 20f;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0f;
        BigCircularBarrageFactory.Instance.CreatBarrage(transform.position, Vector3.zero, -90f);
        yield return new WaitForSeconds(0.96f);
        attacking = false;
    }

    public override void SkillAction()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) > attackRange && Vector2.Distance(transform.position, targetPoint.position) < skillRange && attackRandomCount > 5) //两点间的距离，可判断x，y轴
        {
            if (Time.time > nextAttack && !attacking)
            {
                attacking = true;
                DownDeriving = true;
                anim.SetTrigger("Skill");
                nextAttack = Time.time + attackRace;
                StopCoroutine("StartSkill");
                StartCoroutine("StartSkill");
                attackRandomCount = Random.Range(0, 10);
            }
        }
    }

    private IEnumerator StartSkill()
    {
        currentHorizontalSpeed = 8f;
        currentVerticalSpeed = 25f;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !DownDeriving);
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        attacking = false;
    }

    public override void SwitchPoint()
    {
        base.SwitchPoint();
        currentHorizontalSpeed = speed;
    }

    private void SpeedCorrection()
    {
        if (!colDown || attacking)
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

    private IEnumerator SelfBlast()
    {
        GameObject particle = ObjectPool.Instance.GetObject(attackParticlesPrefab);
        particle.transform.position = transform.position;
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
