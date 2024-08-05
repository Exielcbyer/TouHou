using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;

public class MokouActionTwo : Action
{
    private BOSS BOSS;
    private float currentlTime;
    private float interval = 4f;

    public override void OnAwake()
    {
        BOSS = transform.GetComponent<BOSS>();
    }

    public override void OnStart()
    {
        currentlTime = 0;
        StartCoroutine(Attack());
    }

    public override TaskStatus OnUpdate()
    {
        if (currentlTime > 5f)
        {
            return TaskStatus.Success;
        }
        else
        {
            currentlTime += Time.deltaTime;
        }
        return TaskStatus.Running;
    }

    private IEnumerator Attack()
    {
        BOSS.DownDeriving = true;
        yield return new WaitForSeconds(0.1f);
        BOSS.SetSpeed(10, 150);
        yield return new WaitForSeconds(0.05f);
        BOSS.SetSpeed(0, 0);
        yield return new WaitForSeconds(0.5f);
        BOSS.SetSpeed(0, 20);
        yield return new WaitForSeconds(0.5f);
        BOSS.SetSpeed(10, -50);
        yield return new WaitUntil(() => !BOSS.DownDeriving);
        BOSS.SetSpeed(0, 0);
        int index = 1;
        while (index <= 5)
        {
            yield return new WaitForSeconds(0.4f);
            FireColumnarBarrageFactory.Instance.CreatBarrage(transform.position + new Vector3(index * interval, 0, 0), Vector3.zero);
            FireColumnarBarrageFactory.Instance.CreatBarrage(transform.position + new Vector3(-index * interval, 0, 0), Vector3.zero);
            index++;
        }
    }
}
