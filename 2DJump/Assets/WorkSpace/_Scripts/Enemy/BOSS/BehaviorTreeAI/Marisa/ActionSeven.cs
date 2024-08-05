using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

// ¼«ÏÞ»ð»¨
public class ActionSeven : Action
{
    private Vector3 direction;
    private Vector3 starDirection;
    private float angle;
    private float currentTime;
    private float intervalTime = 1.1f;

    private Transform target => GameObject.Find("Player").transform;

    public override void OnStart()
    {
        Filp(target.position);
        direction = new Vector3(transform.localScale.x, 0, 0);
        int index = 0;
        CannonFactory.Instance.CreatBarrage(transform.position, direction, -110f, transform, index);
        CannonFactory.Instance.CreatBarrage(transform.position, direction, -70f, transform, index);
        currentTime = 0;
        intervalTime = 1.1f;
}

    public override TaskStatus OnUpdate()
    {
        if (currentTime > intervalTime) 
        {
            starDirection = transform.GetChild(0).localScale;
            angle = Random.Range(0, 360);
            StarBarrageFactory.Instance.CreatBarrage(transform.position, starDirection, angle, null, 1);
            angle = Random.Range(0, 360);
            StarBarrageFactory.Instance.CreatBarrage(transform.position, starDirection, angle, null, 1);
            angle = Random.Range(0, 360);
            StarBarrageFactory.Instance.CreatBarrage(transform.position, starDirection, angle);
            intervalTime += 0.1f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }

        if (currentTime > 16f)
        {
            return TaskStatus.Success;
        }
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
