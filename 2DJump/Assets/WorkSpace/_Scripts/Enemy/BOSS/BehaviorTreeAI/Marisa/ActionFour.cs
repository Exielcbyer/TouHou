using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

// 魔理沙飞至地图中心的上方，向地图下方持续发射大量星形弹幕，期间辅助火炮会朝玩家发射火炮。
public class ActionFour : Action
{
    private Vector3 starDirection;
    private Vector3 artilleryDirection;
    private float artilleryAngle;
    private float angle;
    private int startBarrageCount = 0;
    private float currentTime;
    private float intervalTime = 2.1f;
    private Vector3 speed;
    public float maxSpeed = 12f;

    private Transform Artillery1;
    private Transform Artillery2;
    private Vector3[] originPos = new Vector3[2];
    [SerializeField] private Vector3[] targetPos = new Vector3[2];
    private float artilleryIntervalTime;

    private Transform target => GameObject.Find("Player").transform;

    public override void OnAwake()
    {
        Artillery1 = transform.GetChild(1);
        Artillery2 = transform.GetChild(2);
    }

    public override void OnStart()
    {
        originPos[0] = Artillery1.position;
        originPos[1] = Artillery2.position;
        startBarrageCount = 0;
        currentTime = 0;
        intervalTime = 2.1f;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime < 2f)
        {
            Artillery1.position = Vector3.SmoothDamp(Artillery1.position, targetPos[0], ref speed, Time.deltaTime, maxSpeed);
            Artillery2.position = Vector3.SmoothDamp(Artillery2.position, targetPos[1], ref speed, Time.deltaTime, maxSpeed);
        }
        if (currentTime > intervalTime) 
        {
            if (startBarrageCount < 200)
            {
                starDirection = Vector3.down;
                angle = Random.Range(-60, 60);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, starDirection, angle);
                startBarrageCount++;
                angle = Random.Range(-60, 60);
                StarBarrageFactory.Instance.CreatBarrage(transform.position, starDirection, angle);
                startBarrageCount++;
            }
            artilleryDirection = -Artillery1.up;
            artilleryAngle = Vector3.SignedAngle(artilleryDirection, Vector3.right, Vector3.back);
            CircularBarrageFactory.Instance.CreatBarrage(Artillery1.position, artilleryDirection, artilleryAngle);
            artilleryDirection = -Artillery2.up;
            artilleryAngle = Vector3.SignedAngle(artilleryDirection, Vector3.right, Vector3.back);
            CircularBarrageFactory.Instance.CreatBarrage(Artillery2.position, artilleryDirection, artilleryAngle);
            intervalTime += 0.1f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }

        if (startBarrageCount >= 200)
        {
            Artillery1.position = Vector3.SmoothDamp(Artillery1.position, originPos[0], ref speed, Time.deltaTime, maxSpeed);
            Artillery2.position = Vector3.SmoothDamp(Artillery2.position, originPos[1], ref speed, Time.deltaTime, maxSpeed);
            if (Mathf.Abs(originPos[0].x - Artillery1.position.x) < 0.1f && Mathf.Abs(originPos[1].x - Artillery2.position.x) < 0.1f)
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
