using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class StraightLineAction : Action
{
    private Vector3 speed;
    public float maxSpeed = 10f;
    private Vector3 direction;
    private float angle;
    private float currentTime;
    private float intervalTime = 0;
    private int sign = 0;
    private Vector3 targetPos;
    private Transform target => GameObject.Find("Player").transform;

    [SerializeField] private MapName mapName;
    [SerializeField] MapDetailsList_SO mapDetailsList_SO;
    private MapDetails mapDetails => mapDetailsList_SO.GetSoundDeatils(mapName);

    public override void OnStart()
    {
        sign = 0;
        while (sign == 0)
        {
            sign = Random.Range(-1, 2);
        }
        targetPos = transform.position + new Vector3(Random.Range(2f, 4f) * sign, Random.Range(1, 2f), 0);
        if (targetPos.x < mapDetails.leftBorder || targetPos.x > mapDetails.rightBorder)
            targetPos = transform.position + new Vector3(Random.Range(2f, 4f) * -sign, Random.Range(1, 2f), 0);

        intervalTime = 0;
        currentTime = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime < 0.5f && currentTime > intervalTime)  
        {
            direction = (target.position - transform.position).normalized;
            angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back) + Random.Range(-10, 10);
            StraightLineFactory.Instance.CreatBarrage(transform.position, direction, angle, target);
            intervalTime += 0.1f;
        }

        if (currentTime < 1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, 2 * Time.deltaTime, maxSpeed);
            currentTime += Time.deltaTime;
        }
        else
        {
            return TaskStatus.Success;
        }

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
