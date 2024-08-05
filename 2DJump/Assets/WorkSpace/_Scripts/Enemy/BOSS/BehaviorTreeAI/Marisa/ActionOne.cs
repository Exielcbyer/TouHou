using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

// ħ��ɳ��ǰ����������������ε�Ļ�����ε�Ļ���ڽӽ����ʱ���٣��ҿ��Ա���ս��������
public class ActionOne : Action
{
    private Vector3 direction;
    private float angle;
    private int barrageCount = 0;
    private float currentTime;
    private Transform target => GameObject.Find("Player").transform;

    public override void OnStart()
    {
        barrageCount = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime > 0.1f)
        {
            direction = transform.GetChild(0).localScale;
            if (direction.x > 0)
            {
                angle = Random.Range(-20, 45);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
                barrageCount++;
                angle = Random.Range(-20, 45);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
                barrageCount++;
            }
            else
            {
                angle = Random.Range(-160, -225);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
                barrageCount++;
                angle = Random.Range(-160, -225);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
                barrageCount++;
            }
            currentTime = 0;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
        //if (intervalTime > 0.5f)
        //{
        //    angle = Random.Range(-210, -150);
        //    StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
        //    angle = Random.Range(-210, -150);
        //    StarBarrageFactory.Instance.CreatBarrage(transform.position, direction, angle);
        //    intervalTime = 0;
        //}
        //else
        //{
        //    intervalTime += Time.deltaTime;
        //}
        if (barrageCount >= 200)
            return TaskStatus.Success;

        Filp(target.position);

        return TaskStatus.Running;
    }

    private void Filp(Vector3 targetPos)
    {
        if (transform.position.x < targetPos.x)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }
    }
}
