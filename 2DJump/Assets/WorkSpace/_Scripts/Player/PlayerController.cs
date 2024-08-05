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

    [Header("��ײ")]
    [SerializeField] private Bounds characterBounds;// ��ײ��
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int detectorCount = 3;// ���������
    [SerializeField] private float detectionRayLength = 0.1f;// ���������
    [SerializeField] [Range(0.1f, 0.3f)] private float rayBuffer = 0.1f;// ��ֹ��������ײ������

    //���������ĸ��������ײ�����
    private RayRange raysUp, raysRight, raysDown, raysLeft;
    private bool colUp, colRight, colDown, colLeft;

    private float timeLeftGrounded;

    [Header("�ƶ�")]
    [SerializeField] private float acceleration = 90;// ���ٶ�
    [SerializeField] private float moveClamp = 12;// ����ٶ�
    [SerializeField] private float deAcceleration = 60f;// ���ټ��ٶ�
    [SerializeField] private float apexBonus = 2;// ��Ծ���㽱���ٶ�

    [Header("����")]
    [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 80f;// ��С�����ٶ�
    [SerializeField] private float maxFallSpeed = 120f;// ��������ٶ�
    private float fallAcceleration;

    [Header("��Ծ")]
    [SerializeField] private float jumpHeight = 25;// ��Ծ�߶�
    [SerializeField] private float jumpApexThreshold = 10f;// ��Ծ������ֵ
    [SerializeField] private float coyoteTimeThreshold = 0.1f;// ����ʱ������ʱ��
    [SerializeField] private float jumpBuffer = 0.1f;// ��Ծ��������ʱ��
    [SerializeField] private float jumpEndEarlyGravityModifier = 3;// С�������׶���������
    private bool coyoteUsable;
    private bool endedJumpEarly = true;
    private float apexPoint;// ����Ծ�����Ϊ1
    private float lastJumpPressed;
    // ����ʱ�䣬����������ٿص�����,�ܹ����뿪ƽ̨��һ��ʱ����,����ִ����������
    private bool CanUseCoyote => coyoteUsable && !colDown && timeLeftGrounded + coyoteTimeThreshold > Time.time;
    // ��Ծ���壬���ڽ�ɫ������أ������䶯����δ����ʱ������������Ծָ��ᱻ�����������������ʱִ��
    private bool HasBufferedJump => colDown && lastJumpPressed + jumpBuffer > Time.time;

    [Header("λ��")]
    [SerializeField, Tooltip("��߸�ֵ������������Ϊ���������ײ����")]
    private int freeColliderIterations = 10;

    [Header("���")]
    private float dashSpeed = 40f;// ����ٶ�
    private float dashSpeedInAir = 60f;// ���г���ٶ�

    [Header("����")]
    public int comboStep;
    public int convoluteStep;
    private bool attackOver = true;// AttackingThisFrameָʾ���幥��������AttackOverָʾ�����Ĺ�����ҡ
    private float correctionSpeed = 10f;// ��ս���������ٶ�
    private float recoilSpeed = 30f;// Զ�̹����������ٶ�
    private float attackDashSpeed = 180f;// ˲ն�ٶ�
    private float gravityDiscounts = 0.05f;// �����ۿ�
    private float maxSpeedDiscounts = 0.1f;// ����ٶ��ۿ�
    private float holdAttackTime;// ������ս������ʱ��
    private float holdRangedTime;// ����Զ�̹�����ʱ��
    private Transform targetTransform;// ����Ŀ��
    private int lockIndex = 0;// ����Ŀ�����

    [Header("����")]
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

        // ���Ա�������
        if (UnityEngine.Input.GetKeyDown(KeyCode.V))
        {
            if (!skillObject.activeSelf)
            {
                skillObject.transform.position = transform.position;
                skillObject.SetActive(true);
            }
        }
        // ���Ե�Ļ����
        if (UnityEngine.Input.GetKeyDown(KeyCode.F))
        {
            if (!interactTestObject.activeSelf)
                interactTestObject.SetActive(true);
        }
        // ����
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
        // �������߷�Χ. 
        CalculateRayRanged();

        // ������
        LandingThisFrame = false;
        var groundedCheck = RunDetection(raysDown);
        if (colDown && !groundedCheck)
        {
            timeLeftGrounded = Time.time; // ��¼�뿪�����ʱ��
        }
        else if (!colDown && groundedCheck)
        {
            endedJumpEarly = true;
            DownDeriveThisFrame = false;
            coyoteUsable = true; // ��ײ������ʱ����������ʱ������ж�
            LandingThisFrame = true;
        }

        colDown = groundedCheck;

        // ����������
        colUp = RunDetection(raysUp);
        colLeft = RunDetection(raysLeft);
        colRight = RunDetection(raysRight);

        // ���߼��հ��������⵽�����򷵻�true
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
                // ����ˮƽ�ƶ��ٶ�
                currentHorizontalSpeed += Input.X * acceleration * Time.deltaTime;

                // clamped by max frame movement
                currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

                //����Ծ����ʱ��X����������Ӷ��㽱���ٶ�
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
                // �����С����������ʱ����������ٶ�
                var fallAcceleration = !attackOver || endedJumpEarly && currentVerticalSpeed > 0 ? 
                    this.fallAcceleration * jumpEndEarlyGravityModifier : this.fallAcceleration;
                // �����ڼ��ܵ�������Ч������
                fallAcceleration = !attackOver && !DownDeriveThisFrame && currentVerticalSpeed < 0 ? 
                    this.fallAcceleration * gravityDiscounts : fallAcceleration;

                // ����
                currentVerticalSpeed -= fallAcceleration * Time.deltaTime;

                // Clamp�������ڼ�����ٶȼ�С
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
            // Gets stronger the closer to the top of the jump�������Ծ���������������ٶȾ͸�ǿ
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

            // �����ɿ���С��
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

        // �����Զ���ƶ������û�м�⵽��ײ�壬�ƶ��������ж�����
        var hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
        if (!hit)
        {
            transform.position += move;
            return;
        }

        // ����ӵ�ǰλ�õ����������������ƶ�����ӽ���λ��
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
    // ��ս����
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
                // ��������
                if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
                {
                    UpwardDeriveThisFrame = true;
                    StartCoroutine("StartUpwardDerive");
                }
                else
                {
                    // �����չ�
                    //StartCoroutine(SpeedCorrection(comboStep));
                    StartCoroutine("StartAttack");
                }
            }
            else
            {
                StopCoroutine("StartAttackInAir");
                StopCoroutine("StartUpwardDerive");
                // ��������
                if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
                {
                    DownDeriveThisFrame = true;
                    StartCoroutine("StartDownDerive");
                }
                else
                {
                    // �����չ�
                    if (comboStep < 3)
                    {
                        StartCoroutine("StartAttackInAir");
                    }
                    else if (comboStep == 3)
                    {
                        // ����˲ն
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

        // ��������
        if (UnityEngine.Input.GetKey(KeyCode.X) && !UnityEngine.Input.GetKey(KeyCode.Z))
        {
            holdAttackTime += Time.deltaTime;
        }
        // �ɿ�ʹ�����⹥��
        if (UnityEngine.Input.GetKeyUp(KeyCode.X))
        {
            if (holdAttackTime > 0.5f && !SwordDancingThisFrame && !AttackingThisFrame && !DashingThisFrame && !AirDashingThisFrame && !GuardingThisFrame && !ReceiveThisFrame && !RangedAttackingThisFrame) 
            {
                // ��Ӱ����
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
        // ����ˮƽ�����ٶ�
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

        // ������ֱ�����ٶ�
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
        // ���ò����ٶȣ���շ�
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
        // ������ֱ�����ٶ�
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

    // Զ�̹���
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
            // ˫ӥ����
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
        if (playerDetails.hasConvolute && convoluteStep < 3)// ���յ��˶���
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

