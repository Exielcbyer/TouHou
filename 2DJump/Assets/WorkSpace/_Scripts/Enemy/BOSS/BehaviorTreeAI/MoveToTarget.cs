using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToTarget : Action
{
    private Vector3 speed;
    private float distance;
    public float maxSpeed = 20f;

    public bool isEdge;
    public Vector3 targetPos;

    [SerializeField] private MapName mapName;
    [SerializeField] MapDetailsList_SO mapDetailsList_SO;
    private MapDetails mapDetails=> mapDetailsList_SO.GetSoundDeatils(mapName);

   public override void OnStart()
    {
        if (isEdge)
        {
            if (Mathf.Abs(mapDetails.leftBorder - transform.position.x) < Mathf.Abs(mapDetails.rightBorder - transform.position.x))
            {
                targetPos = new Vector3(mapDetails.leftBorder, 3, 0);
                transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                targetPos = new Vector3(mapDetails.rightBorder, 5, 0);
                transform.GetChild(0).localScale = new Vector3(1, 1, 1);
            }
        }
        distance = Vector3.Distance(targetPos, transform.position);
    }

    public override TaskStatus OnUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, distance * 10 * Time.deltaTime, maxSpeed);

        if (Mathf.Abs(targetPos.x - transform.position.x) < 0.1f && Mathf.Abs(targetPos.y - transform.position.y) < 0.1f) 
            return TaskStatus.Success;

        return TaskStatus.Running;
    }
}
