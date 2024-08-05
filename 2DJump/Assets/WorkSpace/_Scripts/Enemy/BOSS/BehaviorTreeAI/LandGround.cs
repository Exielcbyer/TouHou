using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class LandGround : Action
{
    private Vector3 speed;
    private Vector3 targetPos;
    [SerializeField] private float maxSpeed = 12;// 最大速度
    private Transform target => GameObject.Find("Player").transform;

    public override void OnStart()
    {
        targetPos = new Vector3(transform.position.x, -8.7f, 0);
    }

    public override TaskStatus OnUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, Time.deltaTime, maxSpeed);

        if (Mathf.Abs(targetPos.y - transform.position.y) < 0.1f)
        {
            Filp(target.position);
            return TaskStatus.Success;
        }
        else if (transform.position.y < -8.7f)
        {
            Filp(target.position);
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
