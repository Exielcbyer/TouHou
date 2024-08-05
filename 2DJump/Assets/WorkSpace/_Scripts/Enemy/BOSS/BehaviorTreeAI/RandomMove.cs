using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class RandomMove : Action
{
    private Vector3 speed;
    private float maxSpeed = 10;// ����ٶ�
    private int sign = 0;
    private float distance;
    private Vector3 targetPos;
    private Transform target => GameObject.Find("Player").transform;

    [SerializeField] private MapName mapName;
    [SerializeField] MapDetailsList_SO mapDetailsList_SO;
    private MapDetails mapDetails => mapDetailsList_SO.GetSoundDeatils(mapName);

    public override void OnStart()
    {
        if (Mathf.Abs(mapDetails.leftBorder - transform.position.x) < Mathf.Abs(mapDetails.rightBorder - transform.position.x))
        {
            sign = 1;
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else
        {
            sign = -1;
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }
        targetPos = transform.position + new Vector3(Random.Range(8, 16) * sign, 0, 0);
        distance = Vector3.Distance(targetPos, transform.position);
    }

    public override TaskStatus OnUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, distance * 10 * Time.deltaTime, maxSpeed);

        if (Mathf.Abs(targetPos.x - transform.position.x) < 0.1f)
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