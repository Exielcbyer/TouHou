using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

// 魔理沙骑乘扫帚在空中冲刺，期间向地图中发射弹幕
public class ActionTwo : Action
{
    private Vector3 direction;
    private float angle;
    private float leftAngleBorder;
    private float rightAngleBorder;
    private Vector3 offset;
    private float currentTime;
    private float intervalTime = 1.2f;
    private Vector3 speed;
    private int lightRodCount = 0;
    private int randomLightRodCount;
    private int barrageCount = 0;
    public float maxSpeed = 35f;
    public Vector3 targetPos;
    public SharedGameObject[] TransmitPoint = new SharedGameObject[3];

    [SerializeField] private MapName mapName;
    [SerializeField] MapDetailsList_SO mapDetailsList_SO;
    private MapDetails mapDetails => mapDetailsList_SO.GetSoundDeatils(mapName);

    public override void OnStart()
    {
        if (Mathf.Abs(mapDetails.leftBorder - transform.position.x) < Mathf.Abs(mapDetails.rightBorder - transform.position.x))
        {
            targetPos = new Vector3(mapDetails.rightBorder, 5, 0);
            leftAngleBorder = -60f;
            rightAngleBorder = 30f;
            transform.GetChild(0).localScale = new Vector3(1, 1, 1);
        }
        else
        {
            targetPos = new Vector3(mapDetails.leftBorder, 3, 0);
            leftAngleBorder = 60f;
            rightAngleBorder = -30f;
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
        }
        lightRodCount = 0;
        randomLightRodCount = Random.Range(1, 6);
        barrageCount = 0;
        currentTime = 0;
        intervalTime = 1.2f;
    }

    public override TaskStatus OnUpdate()
    {
        if (currentTime > intervalTime && currentTime < 3f)
        {
            direction = Vector3.down;
            angle = Random.Range(leftAngleBorder, rightAngleBorder);
            offset = new Vector3(0, Random.Range(-0.8f, 0.8f), 0);
            StarBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
            angle = Random.Range(leftAngleBorder, rightAngleBorder);
            offset = new Vector3(0, Random.Range(-0.8f, 0.8f), 0);
            StarBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
            angle = Random.Range(leftAngleBorder, rightAngleBorder);
            offset = new Vector3(0, Random.Range(-0.8f, 0.8f), 0);
            StarBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
            angle = Random.Range(leftAngleBorder, rightAngleBorder);
            offset = new Vector3(0, Random.Range(-0.8f, 0.8f), 0);
            StarBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
            angle = Random.Range(leftAngleBorder, rightAngleBorder);
            offset = new Vector3(0, Random.Range(-0.8f, 0.8f), 0);
            StarBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
            if (lightRodCount < 2 && barrageCount == randomLightRodCount) 
            {
                direction = transform.localScale;
                offset = new Vector3(0, Random.Range(-0.5f, 0.5f), 0);
                angle = Random.Range(20, 90);
                LightRodBarrageFactory.Instance.CreatBarrage(transform.position + offset, direction, angle);
                lightRodCount++;
                randomLightRodCount = Random.Range(8, 15);
            }
            intervalTime += 0.1f;
            barrageCount++;
        }
        if (currentTime > 1.2f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, Time.deltaTime, maxSpeed);
        }
        if (currentTime < 4f)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
