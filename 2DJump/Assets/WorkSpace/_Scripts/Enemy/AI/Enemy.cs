using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected EnemyBaseState currentState;
    protected Animation alarmSign;// 惊叹号动画
    public Animator anim;
    public int animState;
    protected SpriteRenderer spriteRenderer;
    public GameObject hitFXPrefab;

    [Header("状态")]
    [SerializeField] protected int enemyID;
    public EnemyDetails_SO enemyDetails_SO;
    protected EnemyDetatils enemyDetails;
    public bool canCheck;// 在巡逻范围才会去检测玩家
    public bool isDead;

    [Header("移动")]
    [SerializeField] protected Transform pointA;
    [SerializeField] protected Transform pointB;
    public float speed;
    public Transform targetPoint;
    public Vector2 distance;

    [Header("攻击")]
    [SerializeField] protected float attackRace;
    [SerializeField] protected float attackRange, skillRange;
    protected bool attacking;
    protected float nextAttack = 0;
    protected int attackRandomCount;
    public Transform attackTarget;

    Vector2 size = new Vector2(1, 1.4f);
    Collider2D[] colliders = new Collider2D[100];

    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        alarmSign = GetComponentInChildren<Animation>();//获取第一个子物体
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        enemyDetails = GetEnemyDetails(enemyID);
        enemyDetails.heath = enemyDetails.maxHeath;
        TransitionToState(patrolState);
    }


    protected virtual void Update()
    {
        if (GameManager.Instance.gameState == GameState.Pause)
        {
            return;
        }
        if (isDead) return;
        if (enemyDetails.heath <= 0)
        {
            isDead = true;
            anim.SetTrigger("Dead");
        }
        
        currentState.OnUpdate();
        anim.SetInteger("State", animState);

        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            if (coll.gameObject.TryGetComponent(out BaseBarrage barrage))
            {
                if (barrage.isFrozen)
                {
                    GetHit(1);
                    barrage.CollEnemy();
                }
            }
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
                ranged.CollEnemy();
            }
        }
    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public virtual void Move()
    {
        if (attackTarget != null)
        {
            if (transform.position.x < attackTarget.position.x)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
        else
        {
            if (transform.position.x < targetPoint.position.x)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        } 
    }

    public virtual void AttackAtion()
    {

    }

    public virtual void SkillAction()// 每个敌人不同
    {

    }

    public virtual void SwitchPoint()
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }

    public virtual void RandomPoint()
    {
    
    }

    private void GetHit(float damage)
    {
        anim.SetTrigger("Hit");
        spriteRenderer.DOColor(Color.red, 1.2f).SetEase(Ease.Flash, 8, 1).OnComplete(() => { spriteRenderer.color = Color.white; });
        transform.DOShakePosition(0.1f, 0.2f, 5, 90);
        var hitFX = ObjectPool.Instance.GetObject(hitFXPrefab);
        hitFX.transform.position = transform.position;
        nextAttack += 0.1f * damage;
        enemyDetails.heath -= damage;
    }

    public EnemyDetatils GetEnemyDetails(int ID)
    {
        return enemyDetails_SO.enemyDetatilsList.Find(i => i.enemyID == ID);
    }

    private IEnumerator OnAlarm()
    {
        alarmSign.Play();
        yield return new WaitForSeconds(alarmSign.clip.length);
        //alarmSign.SetActive(false);
    }
}
