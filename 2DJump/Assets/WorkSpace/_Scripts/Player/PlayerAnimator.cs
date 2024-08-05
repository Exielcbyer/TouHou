using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class PlayerAnimator : MonoBehaviour 
{
    private Animator anim;
    private IPlayerController player;

    [SerializeField] private LayerMask groundMask;

    [Header("特效")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem launchParticles;
    [SerializeField] private ParticleSystem moveParticles;
    [SerializeField] private ParticleSystem landParticles;


    [Header("倾斜")]
    [SerializeField] private float maxTilt = 5f;// 最大倾斜角
    [SerializeField] private float tiltSpeed = 20f;// 倾斜速度

    [SerializeField, Range(1f, 3f)] private float maxIdleSpeed = 2;// idle动画最大速度
    [SerializeField] private float maxParticleFallSpeed = -40;// 粒子特效下落最大速度

    private bool playerGrounded;
    private ParticleSystem.MinMaxGradient currentGradient;// 当前颜色渐变
    private Vector2 movement;

    void Awake()
    {
        player = GetComponentInParent<IPlayerController>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnEnable()
    {
        moveParticles.Play();
        EventHandler.AddEventListener<float>("PauseFrame", OnPauseFrameEvent);
    }

    private void OnDisable()
    {
        moveParticles.Stop();
        EventHandler.RemoveEventListener<float>("PauseFrame", OnPauseFrameEvent);
    }

    void Update() {
        if (player == null) return;

        // 翻转
        if (player.Input.X != 0) 
            transform.parent.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);

        // 移动时倾斜
        //var targetRotVector = new Vector3(0, 0, Mathf.Lerp(-maxTilt, maxTilt, Mathf.InverseLerp(-1, 1, player.Input.X)));
        //anim.transform.rotation = Quaternion.RotateTowards(anim.transform.rotation, Quaternion.Euler(targetRotVector), tiltSpeed * Time.deltaTime);

        // 因为没有独立的Move动画，所以在移动时修改Idle动画的速度
        anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, maxIdleSpeed, Mathf.Abs(player.Input.X)));
        anim.SetFloat(HorizontalSpeedKey, Mathf.Abs(player.Input.X));
        anim.SetBool(FallKey, player.FallingThisFrame);
        anim.SetBool(DashKey, player.DashingThisFrame);
        anim.SetBool(AirDashKey, player.AirDashingThisFrame);
        anim.SetInteger(ComboStepKey, player.ComboStep);
        anim.SetBool(AttackKey, player.AttackingThisFrame);
        anim.SetBool(AttackOverKey, player.AttackOver);
        anim.SetBool(LaidoKey, player.LaidoingThisFrame);
        anim.SetBool(SwordDanceKey, player.SwordDancingThisFrame);
        anim.SetBool(FuryCutterKey, player.FuryCutteringThisFrame);
        anim.SetBool(DrillKey, player.DrillingThisFrame);
        anim.SetBool(HammerKey, player.HammeringThisFrame);
        anim.SetBool(UpwardDeriveKey, player.UpwardDeriveThisFrame);
        anim.SetBool(DownDeriveKey, player.DownDeriveThisFrame);
        anim.SetBool(GroundedKey, player.Grounded);
        anim.SetBool(GuardKey, player.GuardingThisFrame);
        anim.SetBool(TransmitKey, player.TransmitThisFrame);
        anim.SetBool(ReceiveKey, player.ReceiveThisFrame);
        anim.SetBool(RangedAttackKey, player.RangedAttackingThisFrame);
        anim.SetBool(DeadKey, player.Dead);


        // Splat
        if (player.LandingThisFrame) 
        {
            EventHandler.TriggerEvent("PlayFXSource", (SoundName)(Random.Range(0, 3)));
        }

        // 跳跃特效
        if (player.JumpingThisFrame) 
        {
            anim.SetTrigger(JumpKey);
            //anim.ResetTrigger(GroundedKey);

                // 只有落地时播放特效 (避免土狼时间播放)
            if (player.Grounded) 
            {
                //SetColor(jumpParticles);
                //SetColor(launchParticles);
                jumpParticles.Play();
            }
        }

        // 落地特效
        if (!playerGrounded && player.Grounded) {
            playerGrounded = true;
            moveParticles.Play();
            landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, maxParticleFallSpeed, movement.y);
            //SetColor(landParticles);
            landParticles.Play();
        }
        else if (playerGrounded && !player.Grounded) {
            playerGrounded = false;
            moveParticles.Stop();
        }

        // 检测地面颜色并设置特效颜色
        var groundHit = Physics2D.Raycast(transform.position, Vector3.down, 2, groundMask);
        if (groundHit && groundHit.transform.TryGetComponent(out SpriteRenderer r)) {
            currentGradient = new ParticleSystem.MinMaxGradient(r.color * 0.9f, r.color * 1.2f);
            //SetColor(moveParticles);
        }

        movement = player.RawMovement; // Previous frame movement is more valuable
    }

    private void OnPauseFrameEvent(float pauseTime)
    {
        StopCoroutine("StartPauseFrame");
        StartCoroutine(StartPauseFrame(pauseTime));
    }

    private IEnumerator StartPauseFrame(float pauseTime)
    {
        anim.speed = 0.1f;
        yield return new WaitForSeconds(pauseTime);
        anim.speed = 1f;
    }

    // 根据地面的颜色设置特效的颜色
    void SetColor(ParticleSystem ps) {
        var main = ps.main;
        main.startColor = currentGradient;
    }

    #region Animation Keys

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int FallKey = Animator.StringToHash("Fall");
    private static readonly int DashKey = Animator.StringToHash("Dash");
    private static readonly int AirDashKey = Animator.StringToHash("AirDash");
    private static readonly int HorizontalSpeedKey = Animator.StringToHash("HorizontalSpeed");
    private static readonly int AttackKey = Animator.StringToHash("Attack");
    private static readonly int AttackOverKey = Animator.StringToHash("AttackOver");
    private static readonly int ComboStepKey = Animator.StringToHash("ComboStep");
    private static readonly int LaidoKey = Animator.StringToHash("Laido");
    private static readonly int SwordDanceKey = Animator.StringToHash("SwordDance");
    private static readonly int FuryCutterKey = Animator.StringToHash("FuryCutter");
    private static readonly int DrillKey = Animator.StringToHash("Drill");
    private static readonly int HammerKey = Animator.StringToHash("Hammer");
    private static readonly int UpwardDeriveKey = Animator.StringToHash("UpwardDerive");
    private static readonly int DownDeriveKey = Animator.StringToHash("DownDerive");
    private static readonly int GuardKey = Animator.StringToHash("Guard");
    private static readonly int TransmitKey = Animator.StringToHash("Transmit");
    private static readonly int ReceiveKey = Animator.StringToHash("Receive");
    private static readonly int RangedAttackKey = Animator.StringToHash("RangedAttack");
    private static readonly int DeadKey = Animator.StringToHash("Dead");
    #endregion
}
