using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class AirFloating : Action
{
    private Vector3 speed;
    private Vector3 targetPos;
    [SerializeField] private float maxSpeed = 3;// 最大速度
    [SerializeField] private float height;// 浮空高度

    public override void OnStart()
    {
        targetPos = transform.position + new Vector3(0, height, 0);
    }

    public override TaskStatus OnUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, Time.deltaTime, maxSpeed);

        if (Mathf.Abs(targetPos.y - transform.position.y) < 0.1f)
            return TaskStatus.Success;
        else if(transform.position.y > 5f)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }
}
