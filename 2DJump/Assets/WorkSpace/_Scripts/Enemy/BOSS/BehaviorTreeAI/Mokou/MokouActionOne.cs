using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

public class MokouActionOne : Action
{
    private BOSS BOSS;
    private float intervalTime;
    private float speed = 20f;

    public override void OnAwake()
    {
        BOSS = transform.GetComponent<BOSS>();
    }

    public override void OnStart()
    {
        intervalTime = 0;
        StartCoroutine(Attack());
    }

    public override TaskStatus OnUpdate()
    {
        if (intervalTime > 2f)
        {
            return TaskStatus.Success;
        }
        else
        {
            intervalTime += Time.deltaTime;
        }
        return TaskStatus.Running;
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.1f);
        BOSS.SetSpeed(speed, 0);
        yield return new WaitForSeconds(0.05f);
        BOSS.SetSpeed(0, 0);
        yield return new WaitForSeconds(0.5f);
        BOSS.SetSpeed(speed, 0);
        yield return new WaitForSeconds(0.05f);
        BOSS.SetSpeed(0, 0);
        yield return new WaitForSeconds(0.7f);
        BOSS.SetSpeed(speed, 0);
        BigCircularBarrageFactory.Instance.CreatBarrage(transform.position, transform.localScale);
        yield return new WaitForSeconds(0.05f);
        BOSS.SetSpeed(0, 0);
    }
}
