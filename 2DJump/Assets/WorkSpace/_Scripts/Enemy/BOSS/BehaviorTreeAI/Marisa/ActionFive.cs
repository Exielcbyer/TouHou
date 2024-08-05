using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

// 魔理沙骑乘扫帚，期间向地图中抛出多个星形地雷（机制同方形弹幕），随后辅助火炮也会发射出射线进行扫射，扫射到地雷时会将其引爆。
public class ActionFive : Action
{
    private Vector3 direction;
    private float angle;
    private float currentTime;
    private float intervalTime = 0.2f;
    private int barrageCount = 0;
    public SharedGameObject[] TransmitPoint = new SharedGameObject[3];

    private Transform target => GameObject.Find("Player").transform;

    public override void OnStart()
    {
        Filp(target.position);
        direction = new Vector3(transform.localScale.x, 0, 0);
        barrageCount = 0;
        currentTime = 0;
        intervalTime = 0.2f;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime > intervalTime)
        {
            for (int i = 0; i < TransmitPoint.Length; i++)
            {
                direction = TransmitPoint[i].Value.transform.right;
                if (barrageCount % 4 == 0)
                    SquareBarrageFactory.Instance.CreatBarrage(TransmitPoint[i].Value.transform.position, direction, 0, null, barrageCount++);
                else if (barrageCount % 2 == 0)
                {
                    angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
                    CircularBarrageFactory.Instance.CreatBarrage(TransmitPoint[i].Value.transform.position, direction, angle, null, barrageCount++);
                }
                else
                {
                    angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
                    CircularBarrageFactory.Instance.CreatBarrage(TransmitPoint[i].Value.transform.position, direction, angle, null, 0);
                    barrageCount++;
                }
            }
            intervalTime += 0.1f;
        }
        if (currentTime < 4.2f)
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
