using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class ActionDash : Action
{
    //private BOSS BOSS;
    private Vector3 speed;
    public float maxSpeed = 15f;
    private float distance;
    //private float dashDistance = 10f;
    private float stopDistance = 3f;
    //private float dashSpeed = 30f;
    private bool dashOver;
    private Transform target => GameObject.Find("Player").transform;
    private Vector3 dashTargetPos;

    //public override void OnAwake()
    //{
    //    BOSS = transform.GetComponent<BOSS>();
    //}

    public override void OnStart()
    {
        dashOver = false;
    }

    public override TaskStatus OnUpdate()
    {
        //if (Mathf.Abs(target.position.x - transform.position.x) > dashDistance) 
        //    transform.position = Vector3.SmoothDamp(transform.position, dashTargetPos, ref speed, 10 * Time.deltaTime, maxSpeed);
        //else
        //    StartCoroutine(Dash());
        if (transform.position.x < target.position.x)
            dashTargetPos = new Vector3(target.position.x - stopDistance, transform.position.y, target.position.z);
        else
            dashTargetPos = new Vector3(target.position.x + stopDistance, transform.position.y, target.position.z);
        distance = Vector3.Distance(dashTargetPos, transform.position);
        if (Mathf.Abs(transform.position.x - dashTargetPos.x) > 0.1f)
            transform.position = Vector3.SmoothDamp(transform.position, dashTargetPos, ref speed, distance * 10 * Time.deltaTime, maxSpeed);
        else
            dashOver = true;

        if (dashOver)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    //private IEnumerator Dash()
    //{
    //    BOSS.SetSpeed(dashSpeed, 0);
    //    yield return new WaitUntil(() => Mathf.Abs(target.position.x - transform.position.x) < stopDistance);
    //    BOSS.SetSpeed(0, 0);
    //    dashOver = true;
    //}
}
