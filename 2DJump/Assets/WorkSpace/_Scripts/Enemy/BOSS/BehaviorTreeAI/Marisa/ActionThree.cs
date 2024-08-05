using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

// 魔理沙飞至地图中心的上方，随后发射出射线，向四周进行数轮扫射
public class ActionThree : Action
{
    private Vector3 direction;
    private float currentTime;

    private Transform target => GameObject.Find("Player").transform;

    public override void OnStart()
    {
        Filp(target.position);
        direction = new Vector3(transform.localScale.x, 0, 0);
        LaserBarrageFactory.Instance.CreatBarrage(transform.position, direction, -120f, null, -30);
        currentTime = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime < 14f)
        {
            currentTime += Time.deltaTime;
        }
        else
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
