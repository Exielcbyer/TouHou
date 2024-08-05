using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class LaserAction : Action
{
    [Header("小型火炮相关")]
    private Vector3 speed;
    private float angle;
    private bool transmitOver;
    public float maxSpeed = 20f;

    public bool isBack;

    private float currentTime;
    [SerializeField] private float moveTime = 2f;// 小型火炮的移动时间
    [SerializeField] private float actionTime = 15f;// 行为总时间

    [SerializeField] private Vector3[] originPos = new Vector3[8];
    private Vector3[] realOriginPos = new Vector3[8];
    [SerializeField] private Vector3[] targetPos = new Vector3[8];
    private Vector3[] realTargetPos = new Vector3[8];
    [SerializeField] private SharedTransform SmallArtilleries;
    private Transform[] SmallArtilleryList = new Transform[8];
    private Vector3[] dirction = new Vector3[8];
    private float[] distance = new float[8];
    private int halfLength;

    [Header("魔理沙相关")]
    private Vector3 moveTargetPos;

    private Transform target => GameObject.Find("Player").transform;

    public override void OnAwake()
    {
        for (int i = 0; i < SmallArtilleryList.Length; i++)
        {
            SmallArtilleryList[i] = SmallArtilleries.Value.GetChild(i);
        }
        halfLength = SmallArtilleryList.Length / 2;
    }

    public override void OnStart()
    {
        for (int i = 0; i < SmallArtilleryList.Length; i++)
        {
            realOriginPos[i] = originPos[i] + transform.position;
            if (!isBack)
                SmallArtilleryList[i].position = realOriginPos[i];
            SmallArtilleryList[i].gameObject.SetActive(true);
            realTargetPos[i] = targetPos[i] + transform.position;
            dirction[i] = (realTargetPos[i] - transform.position);
            distance[i] = Vector3.Distance(realTargetPos[i], transform.position);
        }
        transmitOver = false;
        currentTime = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime <= moveTime)
        {
            if (!isBack)
            {
                for (int i = 0; i < SmallArtilleryList.Length; i++)
                {
                    SmallArtilleryList[i].position = Vector3.SmoothDamp(SmallArtilleryList[i].position, realTargetPos[i], ref speed, Time.deltaTime, maxSpeed);
                }
            }
            else
            {
                for (int i = 0; i < SmallArtilleryList.Length; i++)
                {
                    SmallArtilleryList[i].position = Vector3.SmoothDamp(SmallArtilleryList[i].position, realOriginPos[i], ref speed, Time.deltaTime, maxSpeed);
                }
            }
        }

        if (currentTime < actionTime && !isBack)
        {
            currentTime += Time.deltaTime;
            if (currentTime > moveTime)
            {
                for (int i = 0; i < halfLength; i++)
                {
                    SmallArtilleryList[i].position = transform.position + dirction[i].normalized * distance[i];
                    SmallArtilleryList[i].RotateAround(transform.position, Vector3.back, 90 * Time.deltaTime);
                    dirction[i] = SmallArtilleryList[i].position - transform.position;
                }
                for (int i = halfLength; i < SmallArtilleryList.Length; i++)
                {
                    SmallArtilleryList[i].position = transform.position + dirction[i].normalized * distance[i];
                    SmallArtilleryList[i].RotateAround(transform.position, Vector3.forward, 90 * Time.deltaTime);
                    dirction[i] = SmallArtilleryList[i].position - transform.position;
                }

                if (!transmitOver)
                {
                    for (int i = 0; i < SmallArtilleryList.Length; i++)// 一起发射
                    {
                        angle = Random.Range(-100, -80);
                        LaserBarrageFactory.Instance.CreatBarrage(SmallArtilleryList[i].position, Vector3.zero, angle, SmallArtilleryList[i], -90);
                    }
                    transmitOver = true;
                }
                Filp();
                Pursuit();
            }
        }
        else if (currentTime < moveTime && isBack)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            if (isBack)
            {
                for (int i = 0; i < SmallArtilleryList.Length; i++)
                {
                    SmallArtilleryList[i].gameObject.SetActive(false);
                }
            }
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private void Filp()
    {
        if (transform.position.x < target.position.x)
        {
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }
    }

    // X轴追击玩家
    private void Pursuit()
    {
        moveTargetPos = new Vector3(target.position.x, transform.position.y, 0);
        transform.position = Vector3.SmoothDamp(transform.position, moveTargetPos, ref speed, Time.deltaTime, maxSpeed / 4);
    }
}
