using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Faery : Enemy
{
    [SerializeField] private float persistRange;
    private NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        distance = new Vector2(0.1f, 0.1f);
    }

    public override void Move()
    {
        if (!attacking)
        {
            if (attackTarget != null && Vector2.Distance(transform.position, targetPoint.position) < persistRange) 
            {
                var direction = (attackTarget.position - transform.position).normalized;
                transform.position -= direction * speed * Time.deltaTime;
            }
            else
            {
                agent.SetDestination(targetPoint.position);
            }

            base.Move();
        }
    }

    public override void AttackAtion()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange && attackRandomCount <= 7)//两点间的距离，可判断x，y轴
        {
            if (Time.time > nextAttack && !attacking)
            {
                attacking = true;
                anim.SetBool("Attack", true);
                nextAttack = Time.time + attackRace;
                StopCoroutine("StartAttack");
                StartCoroutine(StartAttack(attackTarget));
                attackRandomCount = Random.Range(0, 10);
            }
            else if (Time.time <= nextAttack)
            {
                agent.SetDestination(targetPoint.position);
            }
        }
    }

    private IEnumerator StartAttack(Transform attackTarget)
    {
        int count = 0;
        yield return new WaitForSeconds(1f);
        while (count < 20)
        {
            if (attackTarget != null)
            {
                yield return new WaitForSeconds(0.05f);
                var direction = (attackTarget.position - transform.position).normalized;
                var creatPos = new Vector3(transform.position.x + Random.Range(-4, 4), transform.position.y + Random.Range(-4, 4), 0);
                float angle = Random.Range(0, 360);
                ChaosBarrageFactory.Instance.CreatBarrage(creatPos, direction, angle, attackTarget);
                count++;
            }
            else
                break;
        }
        yield return new WaitForSeconds(1f);
        //EventHandler.TriggerEvent<Transform>("StabEnemy", targetTransform);
        attacking = false;
        anim.SetBool("Attack", false);
    }

    public override void SkillAction()
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange && attackRandomCount > 7)//两点间的距离，可判断x，y轴
        {
            if (Time.time > nextAttack && !attacking)
            {
                attacking = true;
                anim.SetTrigger("Attack");
                nextAttack = Time.time + attackRace;
                StopCoroutine("StartSkill");
                StartCoroutine("StartSkill");
                attackRandomCount = Random.Range(0, 10);
            }
            else if (Time.time <= nextAttack)
            {
                agent.SetDestination(targetPoint.position);
            }
        }
    }

    private IEnumerator StartSkill()
    {
        yield return new WaitForSeconds(1f);
        var direction = (attackTarget.position - transform.position).normalized;
        var angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
        BigCircularBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle, transform);
        yield return new WaitForSeconds(0.5f);
        attacking = false;
        anim.SetBool("Attack", false);
    }

    public override void SwitchPoint()
    {
        targetPoint = RandomPointInLargeScale();
    }

    public override void RandomPoint()
    {
        targetPoint = RandomPointInSmallScale();
    }

    public Transform RandomPointInSmallScale()// 在周围小范围随机设置pointA
    {
        Vector3 randomPoint = new Vector3();
        randomPoint.x = Random.Range(transform.localPosition.x - 0.5f, transform.localPosition.x + 0.5f);
        randomPoint.y = Random.Range(transform.localPosition.y - 0.5f, transform.localPosition.y + 0.5f);
        pointA.localPosition = randomPoint;
        return pointA;
    }

    public Transform RandomPointInLargeScale()// 在固定区域内大范围随机设置pointA
    {
        Vector3 randomPoint = new Vector3();
        randomPoint.x = Random.Range(-4f, 4f);
        randomPoint.y = Random.Range(-3f, 3f);
        pointA.localPosition = randomPoint;
        return pointA;
    }
}
