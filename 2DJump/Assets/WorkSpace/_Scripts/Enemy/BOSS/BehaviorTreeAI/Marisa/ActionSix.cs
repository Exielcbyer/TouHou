using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


// 魔理沙飞至地图中心的上方，使两个辅助火炮分裂成数个小型火炮，小型火炮会在地图中生成大量有预警信息的魔力细线，
// 方向包括横向、纵向、斜向、环形（360°），位置包括自身的上方、下方、左侧、右侧和环绕自身360°。
public class ActionSix : Action
{
    private Vector3 speed;
    private Vector3 direction;
    private bool transmitOver;
    public float maxSpeed = 20f;

    public bool isBack;
    public bool isAround;
    public bool isFirstRandom;
    public bool isRandom;

    private float currentTime;
    [SerializeField] private float moveTime = 2f;// 小型火炮的移动时间
    [SerializeField] private float actionTime = 5.5f;// 行为总时间

    [SerializeField] private Vector3[] originPos = new Vector3[8];
    private Vector3[] realOriginPos = new Vector3[8];
    [SerializeField] private float[] angle = new float[8];
    [SerializeField] private Vector3[] targetPos = new Vector3[8];
    private Vector3[] realTargetPos = new Vector3[8];
    [SerializeField] private SharedTransform SmallArtilleries;
    private Transform[] SmallArtilleryList = new Transform[8];

    private Transform target => GameObject.Find("Player").transform;

    public override void OnAwake()
    {
        for (int i = 0; i < SmallArtilleryList.Length; i++)
        {
            SmallArtilleryList[i] = SmallArtilleries.Value.GetChild(i);
        }
    }

    public override void OnStart()
    {
        for (int i = 0; i < SmallArtilleryList.Length; i++)
        {
            realOriginPos[i] = originPos[i] + transform.position;
            if (!isFirstRandom && !isRandom && !isBack) 
                SmallArtilleryList[i].position = realOriginPos[i];
            SmallArtilleryList[i].gameObject.SetActive(true);
            realTargetPos[i] = targetPos[i] + transform.position;
        }
        if (isRandom)
        {
            targetPos[0] = new Vector3(Random.Range(-21f, -19f), Random.Range(7f, 9f), 0);
            targetPos[1] = new Vector3(Random.Range(-21f, -19f), Random.Range(-2f, 2f), 0);
            targetPos[2] = new Vector3(Random.Range(-2f, 2f), Random.Range(7f, 9f), 0);
            targetPos[3] = new Vector3(Random.Range(-21f, -19f), Random.Range(-7f, -9f), 0);
            targetPos[4] = new Vector3(Random.Range(18f, 20f), Random.Range(7f, 9f), 0);
            targetPos[5] = new Vector3(Random.Range(19f, 21f), Random.Range(6f, 8f), 0);
            targetPos[6] = new Vector3(Random.Range(19f, 21f), Random.Range(-6f, -8f), 0);
            targetPos[7] = new Vector3(Random.Range(18f, 02f), Random.Range(-7f, -9f), 0);
            angle[0] = Random.Range(-20, -90);
            angle[1] = Random.Range(10, -80);
            angle[2] = Random.Range(-60, -120);
            angle[3] = Random.Range(0, 60);
            angle[4] = Random.Range(-100, -150);
            angle[5] = Random.Range(-120, -170);
            angle[6] = Random.Range(-170, -210);
            angle[7] = Random.Range(-210, -260);
            for (int i = 0; i < SmallArtilleryList.Length; i++)
            {
                realTargetPos[i] = targetPos[i];
            }
        }

        Filp(target.position);
        direction = transform.localScale;
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
                if (isAround)
                {
                    for (int i = 0; i < SmallArtilleryList.Length; i++)
                    {
                        SmallArtilleryList[i].RotateAround(transform.position, Vector3.forward, 10 * Time.deltaTime);
                    }
                }
                if (!transmitOver)
                {
                    if (!isAround)
                        StartCoroutine(EachTransmit());// 逐个发射
                    else
                        StartCoroutine(AroundTransmit());// 逐个发射，发射多次
                    transmitOver = true;
                }
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

    private IEnumerator EachTransmit()
    {
        int i = 0;
        while (i < SmallArtilleryList.Length)
        {
            yield return new WaitForSeconds(0.1f);
            direction = new Vector3(SmallArtilleryList[i].localScale.x, 0, 0);
            StraightLineFactory.Instance.CreatBarrage(SmallArtilleryList[i].position, direction, angle[i], target);
            i++;
        }
    }

    private IEnumerator AroundTransmit()
    {
        float angle = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < SmallArtilleryList.Length; j++)
            {
                while (angle < 50)
                {
                    yield return new WaitForSeconds(0.05f);
                    //direction = new Vector3(SmallArtilleryList[i].localScale.x, 0, 0);
                    //StraightLineFactory.Instance.CreatBarrage(SmallArtilleryList[i].position, direction, this.angle[i] + angle, target);
                    //angle += 10;
                    var direction = SmallArtilleryList[j].right;
                    CircularBarrageFactory.Instance.CreatBarrage(SmallArtilleryList[j].position, direction, this.angle[j] + angle, null, -1);
                    angle += 10f;
                }
                angle = 0;
            }
        }
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
