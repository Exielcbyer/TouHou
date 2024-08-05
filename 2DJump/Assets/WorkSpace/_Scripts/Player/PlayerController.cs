using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public Vector3 Velocity { get; private set; }
    public FrameInput Input { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool FallingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public bool DashingThisFrame { get; set; }
    public bool AirDashingThisFrame { get; set; }
    public bool AttackingThisFrame { get; set; }
    public bool LaidoingThisFrame { get; set; }
    public bool SwordDancingThisFrame { get; set; }
    public bool FuryCutteringThisFrame { get; set; }
    public bool HammeringThisFrame { get; set; }
    public bool DrillingThisFrame { get; set; }
    public bool UpwardDeriveThisFrame { get; set; }
    public bool DownDeriveThisFrame { get; set; }
    public bool GuardingThisFrame { get; set; }
    public bool TransmitThisFrame { get; set; }
    public bool ReceiveThisFrame { get; set; }
    public bool RangedAttackingThisFrame { get; set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => colDown;
    public int ComboStep => comboStep;
    public bool AttackOver => attackOver;
    public bool Dead { get; set; }

    private Vector3 lastPosition;
    private float currentHorizontalSpeed, currentVerticalSpeed;

    // This is horrible, but for some reason colliders are not fully established when update starts...
    private bool active;

    [Header("碰撞")]
    [SerializeField] private Bounds characterBounds;// 碰撞体
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int detectorCount = 3;// 检测器数量
    [SerializeField] private float detectionRayLength = 0.1f;// 检测器长度
    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;// 防止侧面检测器撞击地面

    //上下左右四个方向的碰撞检测器
    private RayRange raysUp, raysRight, raysDown, raysLeft;
    private bool colUp, colRight, colDown, colLeft;

    private float timeLeftGrounded;

    [Header("移动")]
    [SerializeField] private float acceleration = 90;// 加速度
    [SerializeField] private float moveClamp = 12;// 最大速度
    [SerializeField] private float deAcceleration = 60f;// 减速加速度
    [SerializeField] private float apexBonus = 2;// 跳跃顶点奖励速度

    [Header("重力")]
    [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 80f;// 最小下落速度
    [SerializeField] private float maxFallSpeed = 120f;// 最大下落速度
    private float fallAcceleration;

    [Header("跳跃")]
    [SerializeField] private float jumpHeight = 25;// 跳跃高度
    [SerializeField] private float jumpApexThreshold = 10f;// 跳跃顶点阈值
    [SerializeField] private float coyoteTimeThreshold = 0.1f;// 土狼时间允许时间
    [SerializeField] private float jumpBuffer = 0.1f;// 跳跃缓冲允许时间
    [SerializeField] private float jumpEndEarlyGravityModifier = 3;// 小跳上升阶段重力修正
    private bool coyoteUsable;
    private bool endedJumpEarly = true;
    private float apexPoint;// 在跳跃顶点变为1
    private float lastJumpPressed;
    // 土狼时间，及让玩家所操控的人物,能够在离开平台的一段时间内,仍能执行起跳操作
    private bool CanUseCoyote => coyoteUsable && !colDown && timeLeftGrounded + coyoteTimeThreshold > Time.time;
    // 跳跃缓冲，及在角色即将落地，但下落动作还未结束时，玩家输入的跳跃指令会被缓存起来，待到落地时执行
    private bool HasBufferedJump => colDown && lastJumpPressed + jumpBuffer > Time.time;

    [Header("位移")]
    [SerializeField, Tooltip("提高该值会以牺牲性能为代价提高碰撞精度")]
    private int freeColliderIterations = 10;

    [Header("冲刺")]
    private float dashSpeed = 40f;// 冲刺速度
    private float dashSpeedInAir = 60f;// 空中冲刺速度

    [Header("攻击")]
    public int comboStep;
    public int convoluteStep;
    private bool attackOver = true;// AttackingThisFrame指示主体攻击动作，AttackOver指示完整的攻击后摇
    private float correctionSpeed = 10f;// 近战攻击补正速度
    private float recoilSpeed = 30f;// 远程攻击后坐力速度
    private float attackDashSpeed = 180f;// 瞬斩速度
    private float gravityDiscounts = 0.05f;// 重力折扣
    private float maxSpeedDiscounts = 0.1f;// 最大速度折扣
    private float holdAttackTime;// 长按近战攻击的时间
    private float holdRangedTime;// 长按远程攻击的时间
    private Transform targetTransform;// 锁定目标
    private int lockIndex = 0;// 锁定目标序号

    [Header("防御")]
    private bool guardOver = true;

    [Header("GameObject")]
    public GameObject attackSlashPrefab;
    //public GameObject spaceWrenchPrefab;
    public GameObject lockingObject;
    public GameObject skillObject;

    public GameObject interactTestObject;
    public GameObject attackAreaObject;
    public GameObject drillingObject;
    public GameObject shieldObject;
    private Shield shield => shieldObject.GetComponent<Shield>();

    public PlayerDetails_SO playerDetails;

    void Awake() => Invoke(nameof(Activate), 0.5f);
    void Activate() => active = true;

    private void Update()
    {
        if (!active || Dead) return;
        if (GameManager.Instance.gameState == GameState.Pause) return;

        if (playerDetails.heath <= 0)
        {
            StartCoroutine(PlayerDead());
        }

        Velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        GatherInput();
        RunCollisionChecks();

        CalculateWalk();
        CalculateJumpApex();
        CalculateGravity();
        CalculateJump();
        CalculateDash();
        CalculateAttack();
        CalculateRanged();
        CalculateGuard();

        MoveCharacter();

        // 测试冰冻技能
        if (UnityEngine.Input.GetKeyDown(KeyCode.V))
        {
            if (!skillObject.activeSelf)
            {
                skillObject.transform.position = transform.position;
                skillObject.SetActive(true);
            }
        }
        // 测试弹幕互动
        if (UnityEngine.Input.GetKeyDown(KeyCode.F))
        {
            if (!interactTestObject.activeSelf)
                interactTestObject.SetActive(true);
        }
        // 锁定
        if (UnityEngine.Input.GetKeyDown(KeyCode.B))
        {
            if (lockIndex < GameManager.Instance.enemyList.Count)
            {
                targetTransform = GameManager.Instance.enemyList[lockIndex++].transform;
                lockingObject.SetActive(true);
                lockingObject.GetComponent<Locking>().LockTarget(targetTransform);
                //EventHandler.TriggerEvent("SwichCamraFollowEvent", targetTransform);
            }
            else
            {
                lockIndex = 0;
                targetTransform = null;
                lockingObject.SetActive(false);
                //EventHandler.TriggerEvent("SwichCamraFollowEvent", transform);
            }
        }
    }


    #region Gather Input
    private void GatherInput()
    {
        Input = new FrameInput
        {
            X = UnityEngine.Input.GetAxisRaw("Horizontal"),
            Y = UnityEngine.Input.GetAxisRaw("Vertical"),
            JumpDown = UnityEngine.Input.GetButtonDown("Jump"),
            JumpUp = UnityEngine.Input.GetButtonUp("Jump"),
            Dash = UnityEngine.Input.GetKeyDown(KeyCode.Space),
        };
        if (Input.JumpDown)
        {
            lastJumpPressed = Time.time;
        }
    }

    #endregion

    #region Collisions
    private void RunCollisionChecks()
    {
        // 生成射线范围. 
        CalculateRayRanged();

        // 地面检测
        LandingThisFrame = false;
        var groundedCheck = RunDetection(raysDown);
        if (colDown && !groundedCheck)
        {
            timeLeftGrounded = Time.time; // 记录离开地面的时刻
        }
        else if (!colDown && groundedCheck)
        {
            endedJumpEarly = true;
            DownDeriveThisFrame = false;
            coyoteUsable = true; // 碰撞到地面时，允许土狼时间进行判定
            LandingThisFrame = true;
        }

        colDown = groundedCheck;

        // 其他方向检测
        colUp = RunDetection(raysUp);
        colLeft = RunDetection(raysLeft);
        colRight = RunDetection(raysRight);

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
        raysUp = new RayRange(b.min.x + rayBuffer, b.max.y, b.max.x - rayBuffer, b.max.y, Vector2.up);
        raysLeft = new RayRange(b.min.x, b.min.y + rayBuffer, b.min.x, b.max.y - rayBuffer, Vector2.left);
        raysRight = new RayRange(b.max.x, b.min.y + rayBuffer, b.max.x, b.max.y - rayBuffer, Vector2.right);
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
            foreach (var range in new List<RayRange> { raysUp, raysRight, raysDown, raysLeft })
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(currentHorizontalSpeed, currentVerticalSpeed) * Time.deltaTime;
        Gizmos.DrawWireCube(transform.position + move, characterBounds.size);
    }

    #endregion

    #region Walk
    private void CalculateWalk()
    {
        if (!DashingThisFrame && !AirDashingThisFrame && attackOver && !GuardingThisFrame)
        {
            if (Input.X != 0)
            {
                // 设置水平移动速度
                currentHorizontalSpeed += Input.X * acceleration * Time.deltaTime;

                // clamped by max frame movement
                currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

                //在跳跃顶点时有X轴输入则添加顶点奖励速度
                var apexBonus = Mathf.Sign(Input.X) * this.apexBonus * apexPoint;
                currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else
            {
                currentHorizontalSpeed = Mathf.MoveTowards(currentHorizontalSpeed, 0, deAcceleration * Time.deltaTime);
            }

        }
        if (currentHorizontalSpeed > 0 && colRight || currentHorizontalSpeed < 0 && colLeft)
        {
            currentHorizontalSpeed = 0;
        }
    }
    #endregion

    #region Gravity
    private void CalculateGravity()
    {
        if (!AirDashingThisFrame)
        {
            if (colDown)
            {
                if (currentVerticalSpeed < 0)
                    currentVerticalSpeed = 0;
            }
            else
            {
                // 如果是小跳，在上升时增大下落加速度
                var fallAcceleration = !attackOver || endedJumpEarly && currentVerticalSpeed > 0 ? 
                    this.fallAcceleration * jumpEndEarlyGravityModifier : this.fallAcceleration;
                // 攻击期间受到的重力效果打折
                fallAcceleration = !attackOver && !DownDeriveThisFrame && currentVerticalSpeed < 0 ? 
                    this.fallAcceleration * gravityDiscounts : fallAcceleration;

                // 下落
                currentVerticalSpeed -= fallAcceleration * Time.deltaTime;

                // Clamp，攻击期间最大速度减小
                if (attackOver || DownDeriveThisFrame)
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
    }
    #endregion

    #region Jump
    private void CalculateJumpApex()
    {
        if (!colDown)
        {
            // Gets stronger the closer to the top of the jump如果离跳跃顶点更近，下落加速度就更强
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            fallAcceleration = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }
    }

    private void CalculateJump()
    {
        if (!AttackingThisFrame && !DashingThisFrame && !DownDeriveThisFrame && !GuardingThisFrame && !ReceiveThisFrame && !RangedAttackingThisFrame)
        {
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                InterruptAttack();
                currentVerticalSpeed = jumpHeight;
                endedJumpEarly = false;
                coyoteUsable = false;
                timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // 马上松开则小跳
            if (!colDown && Input.JumpUp && !endedJumpEarly && Velocity.y > 0)
            {
                //currentVerticalSpeed = 0;
                endedJumpEarly = true;
            }
        }

        if (colUp)
        {
            if (currentVerticalSpeed > 0)
                currentVerticalSpeed = 0;
        }

        if (!colDown && currentVerticalSpeed < 0)
            FallingThisFrame = true;
        else
            FallingThisFrame = false;
    }

    #endregion

    #region Dash
    private void CalculateDash()
    {
        if (Input.Dash && !DashingThisFrame && !AttackingThisFrame && !DownDeriveThisFrame && !ReceiveThisFrame && !RangedAttackingThisFrame)
        {
            InterruptAttack();
            if (colDown)
            {
                if (colRight || colLeft)
                {
                    currentHorizontalSpeed = 0;
                }
                else
                {
                    float speed = currentHorizontalSpeed;
                    StartCoroutine(StartDash(speed));
                }
            }
            else
            {
                if (playerDetails.energy >= 0f)
                {
                    if (colRight || colLeft)
                    {
                        StartCoroutine(StartAirDash());
                        currentHorizontalSpeed = 0;
                    }
                    else
                    {
                        StartCoroutine(StartAirDash());
                    }
                    playerDetails.energy -= 5;
                    EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
                }
            }
        }
    }

    private IEnumerator StartDash(float speed)
    {
        DashingThisFrame = true;
        if (Input.X != 0)
        {
            currentHorizontalSpeed = Input.X * dashSpeed;
        }
        else
        {
            currentHorizontalSpeed = transform.localScale.x * dashSpeed;
        }
        if (GuardingThisFrame)
            yield return new WaitForSeconds(0.15f);
        else
            yield return new WaitForSeconds(0.25f);
        currentHorizontalSpeed = speed;
        DashingThisFrame = false;
    }

    private IEnumerator StartAirDash()
    {
        AirDashingThisFrame = true;
        if (Input.X != 0 && Input.Y != 0)
        {
            currentHorizontalSpeed = Input.X * (dashSpeedInAir / 1.414f);
            currentVerticalSpeed = Input.Y * (dashSpeedInAir / 1.414f);
        }
        else if (Input.X != 0)
        {
            currentHorizontalSpeed = Input.X * dashSpeedInAir;
        }
        else if (Input.Y != 0)
        {
            currentVerticalSpeed = Input.Y * dashSpeedInAir;
        }
        else
        {
            currentHorizontalSpeed = transform.localScale.x * dashSpeedInAir;
        }

        yield return new WaitForSeconds(0.1f);
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        yield return new WaitForSeconds(0.1f);
        AirDashingThisFrame = false;
    }
    #endregion

    #region Move
    private void MoveCharacter()
    {
        var pos = transform.position;
        RawMovement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed);
        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // 检查最远的移动。如果没有检测到碰撞体，移动，不进行额外检查
        var hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
        if (!hit)
        {
            transform.position += move;
            return;
        }

        // 否则从当前位置递增；看看我们能移动到最接近的位置
        var positionToMoveTo = transform.position;
        for (int i = 1; i < freeColliderIterations; i++)
        {
            var t = (float)i / freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, characterBounds.size, 0, groundLayer))
            {
                transform.position = positionToMoveTo;
                if (i == 1)
                {
                    if (currentVerticalSpeed < 0) currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    transform.position += dir.normalized * move.magnitude;
                }
                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion

    #region Attack
    // 近战攻击
    private void CalculateAttack()
    {
        attackAreaObject.SetActive(AttackingThisFrame);
        if (UnityEngine.Input.GetKeyDown(KeyCode.X) && !AttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame && !GuardingThisFrame && !ReceiveThisFrame && !RangedAttackingThisFrame)
        {
            AttackingThisFrame = true;
            attackOver = false;
            comboStep++;
            if (comboStep > 3)
                comboStep = 1;
            if (colDown)
            {
                StopCoroutine("StartAttack");
                StopCoroutine("StartDownDerive");
                // 上挑派生
                if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
                {
                    UpwardDeriveThisFrame = true;
                    StartCoroutine("StartUpwardDerive");
                }
                else
                {
                    // 地面普攻
                    //StartCoroutine(SpeedCorrection(comboStep));
                    StartCoroutine("StartAttack");
                }
            }
            else
            {
                StopCoroutine("StartAttackInAir");
                StopCoroutine("StartUpwardDerive");
                // 下砸派生
                if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
                {
                    DownDeriveThisFrame = true;
                    StartCoroutine("StartDownDerive");
                }
                else
                {
                    // 空中普攻
                    if (comboStep < 3)
                    {
                        StartCoroutine("StartAttackInAir");
                    }
                    else if (comboStep == 3)
                    {
                        // 空中瞬斩
                        LaidoingThisFrame = true;
                        if (colRight || colLeft)
                        {
                            StartCoroutine(StartAttackDash());
                            currentHorizontalSpeed = 0;
                        }
                        else
                        {
                            StartCoroutine(StartAttackDash());
                        }
                    }
                }
            }
        }

        // 长按蓄力
        if (UnityEngine.Input.GetKey(KeyCode.X) && !UnityEngine.Input.GetKey(KeyCode.Z))
        {
            holdAttackTime += Time.deltaTime;
        }
        // 松开使用特殊攻击
        if (UnityEngine.Input.GetKeyUp(KeyCode.X))
        {
            if (holdAttackTime > 0.5f && !SwordDancingThisFrame && !AttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame && !GuardingThisFrame && !ReceiveThisFrame && !RangedAttackingThisFrame) 
            {
                // 幻影剑舞
                if (!playerDetails.hasIceHammer && !playerDetails.hasDrillingBit)
                {
                    AttackingThisFrame = true;
                    attackOver = false;
                    StopCoroutine("StartAttack");
                    StopCoroutine("StartAttackInAir");
                    StartCoroutine(StartSwordDance(playerDetails.enhancementPoints > 0));
                    EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
                    currentHorizontalSpeed = 0;
                }
                else if (playerDetails.hasIceHammer)
                {
                    AttackingThisFrame = true;
                    attackOver = false;
                    HammeringThisFrame = true;
                    StopCoroutine("StartAttack");
                    StopCoroutine("StartAttackInAir");
                    StartCoroutine("StartHammering");
                    playerDetails.hasIceHammer = false;
                    currentHorizontalSpeed = 0;
                }
                else if (playerDetails.hasDrillingBit && targetTransform)
                {
                    attackOver = false;
                    DrillingThisFrame = true;
                    StopCoroutine("StartAttack");
                    StopCoroutine("StartAttackInAir");
                    StartCoroutine(StartDrilling(playerDetails.enhancementPoints > 0));
                    playerDetails.enhancementPoints--;
                    EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
                    playerDetails.hasDrillingBit = false;
                    EventHandler.TriggerEvent<bool>("UpdateDrillingBitEvent", playerDetails.hasDrillingBit);
                    currentHorizontalSpeed = 0;
                }
            }
            holdAttackTime = 0;
        }
        if (DrillingThisFrame && AttackingThisFrame)
        {
            DrillingDash();
        }
    }

    private IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(0.25f);
        // 设置水平补正速度
        currentHorizontalSpeed = correctionSpeed * Input.X * comboStep;
        currentVerticalSpeed = 0;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0;
        yield return new WaitForSeconds(0.1f);
        AttackingThisFrame = false;
        yield return new WaitForSeconds(0.2f);
        attackOver = true;
        comboStep = 0;
    }

    private IEnumerator StartAttackInAir()
    {
        yield return new WaitForSeconds(0.25f);
        currentHorizontalSpeed = 0;

        // 设置竖直补正速度
        currentVerticalSpeed = correctionSpeed * comboStep;
        yield return new WaitForSeconds(0.15f);
        AttackingThisFrame = false;
        yield return new WaitForSeconds(0.2f);
        attackOver = true;
        comboStep = 0;
    }

    private IEnumerator StartAttackDash()
    {
        AirDashingThisFrame = true;
        var attackParticles = ObjectPool.Instance.GetObject(attackSlashPrefab);
        attackParticles.transform.position = transform.position;
        attackParticles.GetComponent<AttackSlashParticles>().Init(Input.X, Input.Y, transform.localScale.x);

        if (Input.X != 0 && Input.Y != 0)
        {
            currentHorizontalSpeed = Input.X * (attackDashSpeed / 1.414f);
            currentVerticalSpeed = Input.Y * (attackDashSpeed / 1.414f);
        }
        else if (Input.X != 0)
        {
            currentHorizontalSpeed = Input.X * attackDashSpeed;
        }
        else if (Input.Y != 0)
        {
            currentVerticalSpeed = Input.Y * attackDashSpeed;
        }
        else
        {
            currentHorizontalSpeed = transform.localScale.x * attackDashSpeed;
        }

        yield return new WaitForSeconds(0.03f);
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        yield return new WaitForSeconds(0.4f);
        AttackingThisFrame = false;
        attackOver = true;
        LaidoingThisFrame = false;
        AirDashingThisFrame = false;
    }

    private IEnumerator StartSwordDance(bool isEnhancement)
    {
        if (isEnhancement)
        {
            SwordDancingThisFrame = true;
            playerDetails.enhancementPoints--;
            attackAreaObject.GetComponent<AttackArea>().isMulti = true;
            if (!colDown)
            {
                int count = 0;
                while (count < 10)
                {
                    yield return new WaitForSeconds(0.24f);
                    currentVerticalSpeed = 10f;
                    count++;
                }
            }
            else
                yield return new WaitForSeconds(2.4f);
            SwordDancingThisFrame = false;
        }
        else
        {
            FuryCutteringThisFrame = true;
            attackAreaObject.GetComponent<AttackArea>().isMulti = true;
            if (!colDown)
            {
                int count = 0;
                while (count < 4)
                {
                    yield return new WaitForSeconds(0.3f);
                    currentVerticalSpeed = 10f;
                    count++;
                }
            }
            else
                yield return new WaitForSeconds(1.2f);
            FuryCutteringThisFrame = false;
        }
        AttackingThisFrame = false;
        attackOver = true;
        comboStep = 0;
        attackAreaObject.GetComponent<AttackArea>().isMulti = false;
    }

    private IEnumerator StartHammering()
    {
        yield return new WaitForSeconds(0.3f);
        AttackingThisFrame = false;
        HammeringThisFrame = false;
        yield return new WaitForSeconds(0.15f);
        attackOver = true;
        comboStep = 0;
    }

    private IEnumerator StartDrilling(bool isEnhancement)
    {
        currentHorizontalSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        if (transform.localScale.x > 0)
            transform.right = (targetTransform.position - transform.position).normalized;
        else
            transform.right = -(targetTransform.position - transform.position).normalized;
        if (isEnhancement)
            drillingObject.transform.localScale = new Vector3(4, 4, 1);
        else
            drillingObject.transform.localScale = new Vector3(2, 2, 1);
        drillingObject.SetActive(true);
        AttackingThisFrame = true;
        yield return new WaitForSeconds(1.85f);
        drillingObject.SetActive(false);
        // 设置补正速度，后空翻
        currentHorizontalSpeed = -transform.localScale.x * correctionSpeed;
        currentVerticalSpeed = correctionSpeed;
        yield return new WaitForSeconds(0.4f);
        AttackingThisFrame = false;
        DrillingThisFrame = false;
        transform.right = Vector3.right;
        yield return new WaitForSeconds(0.1f);
        attackOver = true;
        comboStep = 0;
    }

    private void DrillingDash()
    {
        if (targetTransform)
        {
            var distance = Vector3.Distance(transform.position, targetTransform.position);
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, distance * 0.4f * Time.deltaTime);
        }
    }

    private IEnumerator StartUpwardDerive()
    {
        comboStep = 0;
        yield return new WaitForSeconds(0.1f);
        currentHorizontalSpeed = correctionSpeed * Input.X;
        currentVerticalSpeed = 50f;
        yield return new WaitForSeconds(0.15f);
        currentHorizontalSpeed = 0;
        yield return new WaitForSeconds(0.35f);
        // 设置竖直补正速度
        AttackingThisFrame = false;
        UpwardDeriveThisFrame = false;
        yield return new WaitForSeconds(0.2f);
        attackOver = true;
    }

    private IEnumerator StartDownDerive()
    {
        comboStep = 0;
        currentVerticalSpeed = 20f;
        yield return new WaitForSeconds(0.1f);
        currentHorizontalSpeed = correctionSpeed * Input.X;
        currentVerticalSpeed = -30f;
        yield return new WaitUntil(() => !DownDeriveThisFrame);
        currentHorizontalSpeed = 0;
        AttackingThisFrame = false;
        attackOver = true;
    }

    // 远程攻击
    private void CalculateRanged()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Z) && !RangedAttackingThisFrame && !AttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame && !GuardingThisFrame && !ReceiveThisFrame)
        {
            if (playerDetails.energy >= 0f)
            {
                InterruptAttack();
                RangedAttackingThisFrame = true;
                attackOver = false;
                StartCoroutine(StartRanged(RangedType.Icicle));
                playerDetails.energy -= 2;
                EventHandler.TriggerEvent<float, float>("UpdatePlayerEnergy", playerDetails.energy, playerDetails.maxEnergy);
                currentHorizontalSpeed *= 0.5f;
            }
        }
        if (UnityEngine.Input.GetKey(KeyCode.Z) && !UnityEngine.Input.GetKey(KeyCode.X))
        {
            holdRangedTime += Time.deltaTime;
        }
        if (UnityEngine.Input.GetKeyUp(KeyCode.Z))
        {
            if (holdRangedTime > 1f && !RangedAttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame)
            {
                InterruptAttack();
                RangedAttackingThisFrame = true;
                attackOver = false;
                if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
                    StartCoroutine(StartRanged(RangedType.Volley));
                else
                    StartCoroutine(StartRanged(RangedType.IceBall));
            }
            holdRangedTime = 0;
        }
    }

    private IEnumerator StartRanged(RangedType rangedType)
    {
        Vector3 direction;
        float angle;
        float shootTime = 0;
        switch (rangedType)
        {
            case RangedType.Icicle:
                if (targetTransform)
                    direction = (targetTransform.position - transform.position).normalized;
                else
                    direction = new Vector3(transform.localScale.x, 0, 0);
                angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
                shootTime = 0.4f;
                IcicleRangedFactory.Instance.CreatRanged(transform.position, direction, angle);
                IcicleRangedFactory.Instance.CreatRanged(transform.position, direction, angle + 10f);
                IcicleRangedFactory.Instance.CreatRanged(transform.position, direction, angle - 10f);
                currentHorizontalSpeed *= 0.5f;
                yield return new WaitForSeconds(shootTime);
                RangedAttackingThisFrame = false;
                attackOver = true;
                break;
            case RangedType.IceBall:
                direction = new Vector3(transform.localScale.x, 0, 0);
                shootTime = 0.25f;
                IceBallRangedFactory.Instance.CreatRanged(transform.position, direction);
                currentHorizontalSpeed = -direction.x * recoilSpeed;
                yield return new WaitForSeconds(shootTime);
                RangedAttackingThisFrame = false;
                attackOver = true;
                break;
            case RangedType.Volley:
                int count = 0;
                shootTime = 0.2f;
                while (count < 50)
                {
                    direction = new Vector3(transform.localScale.x, 0, 0);
                    angle = 10 * count;
                    VolleyRangedFactory.Instance.CreatRanged(transform.position, direction, angle);
                    currentHorizontalSpeed = 0;
                    count++;
                    yield return new WaitForSeconds(0.01f);
                }
                yield return new WaitForSeconds(shootTime);
                RangedAttackingThisFrame = false;
                attackOver = true;
                yield return new WaitForSeconds(1f);
                EventHandler.TriggerEvent<Transform>("StabEnemy", targetTransform);
                break;
            default:
                break;
        }
    }

    private void InterruptAttack()
    {
        StopCoroutine("StartAttack");
        StopCoroutine("StartAttackInAir");
        StopCoroutine("ReceiveShieldConvolute");
        StopCoroutine("StartRanged");
        AttackingThisFrame = false;
        attackOver = true;
        ReceiveThisFrame = false;
        RangedAttackingThisFrame = false;
        comboStep = 0;
    }
    #endregion

    #region Guard
    private void CalculateGuard()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftAlt) && !AttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame && !GuardingThisFrame && !RangedAttackingThisFrame)
        {
            // 双鹰回旋
            if (shield.isOverheat && convoluteStep < 3) 
            {
                AttackingThisFrame = true;
                attackOver = false;
                shield.isOverheat = false;
                EventHandler.TriggerEvent<bool>("UpdateConvoluteShieldEvent", shield.isOverheat);
                comboStep = 0;
                convoluteStep++;
                ReceiveThisFrame = false;
                TransmitThisFrame = true;
                StopCoroutine("StartAttack");
                StopCoroutine("StartAttackInAir");
                StopCoroutine("ReceiveShieldConvolute");
                StartCoroutine("TransmitShieldConvolute");
                playerDetails.hasIceHammer = false;
                currentHorizontalSpeed = 0;
            }
            else if (playerDetails.enhancementPoints > 0)
            {
                GuardingThisFrame = true;
                StartCoroutine("StartHoldingShield");
                playerDetails.enhancementPoints--;
                EventHandler.TriggerEvent<int>("UpdateEnhancementPointsEvent", playerDetails.enhancementPoints);
            }
        }
        if (playerDetails.hasConvolute && convoluteStep < 3)// 回收到了盾牌
        {
            AttackingThisFrame = false;
            attackOver = false;
            comboStep = 0;
            playerDetails.hasConvolute = false;
            ReceiveThisFrame = true;
            TransmitThisFrame = false;
            StopCoroutine("StartAttack");
            StopCoroutine("StartAttackInAir");
            StopCoroutine("TransmitShieldConvolute");
            StartCoroutine("ReceiveShieldConvolute");
        }
        if (GuardingThisFrame && guardOver && shield.isGuardingLaser) 
        {
            guardOver = false;
            shield.isGuardingLaser = false;
            StopCoroutine("StartHoldingShield");
            StartCoroutine(StartHoldingShieldInLaser());
        }
        if (GuardingThisFrame && shield.isGuarding)
        {
            shield.isGuarding = false;
            StopCoroutine("GuardSpeedCorrection");
            StartCoroutine("GuardSpeedCorrection");
        }
    }

    private IEnumerator TransmitShieldConvolute()
    {
        yield return new WaitForSeconds(0.35f);
        Vector3 direction;
        if (!colDown)
        {
            currentVerticalSpeed = correctionSpeed * 2;
            direction = new Vector3(transform.localScale.x, -0.5f, 0);
            ShieldRangedFactory.Instance.CreatRanged(transform.position, direction, direction.x * -30f, null, convoluteStep);
        }
        else
        {
            direction = new Vector3(transform.localScale.x, 0, 0);
            ShieldRangedFactory.Instance.CreatRanged(transform.position, direction, 0, null, convoluteStep);
        }
        yield return new WaitForSeconds(0.2f);
        AttackingThisFrame = false;
        TransmitThisFrame = false;
        attackOver = true;
        yield return new WaitForSeconds(5f);
        convoluteStep = 0;
    }

    private IEnumerator ReceiveShieldConvolute()
    {
        if (!colDown)
            currentVerticalSpeed = correctionSpeed * 1.5f;
        currentHorizontalSpeed = -transform.localScale.x * 5;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0;
        shield.isOverheat = true;
        EventHandler.TriggerEvent<bool>("UpdateConvoluteShieldEvent", shield.isOverheat);
        yield return new WaitForSeconds(0.5f);
        ReceiveThisFrame = false;
        attackOver = true;
        shield.isOverheat = false;
        EventHandler.TriggerEvent<bool>("UpdateConvoluteShieldEvent", shield.isOverheat);
        convoluteStep = 0;
    }

    private IEnumerator StartHoldingShield()
    {
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        shieldObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        shieldObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        GuardingThisFrame = false;
        guardOver = true;
    }

    private IEnumerator StartHoldingShieldInLaser()
    {
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        EventHandler.TriggerEvent("PauseFrame", 5f);
        shieldObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        shieldObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        GuardingThisFrame = false;
        guardOver = true;
    }

    private IEnumerator GuardSpeedCorrection()
    {
        if (DashingThisFrame || AirDashingThisFrame)
            currentHorizontalSpeed = 0;
        else
            currentHorizontalSpeed = -transform.localScale.x;
        yield return new WaitForSeconds(0.05f);
        currentHorizontalSpeed = 0;
    }
    #endregion

    private IEnumerator PlayerDead()
    {
        Dead = true;
        yield return new WaitForSeconds(2f);
        EventHandler.TriggerEvent("LoadEvent");
        yield return new WaitForSeconds(1f);
        Dead = false;
    }
}

