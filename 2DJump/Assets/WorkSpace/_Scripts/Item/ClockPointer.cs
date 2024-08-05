using System.Collections;
using UnityEngine;

public class ClockPointer : MonoBehaviour
{
    [SerializeField ]private int index;
    private bool isStage2;
    private bool canRotate;
    private float angularSpeed = 20f;
    private float ClockTimer;

    private void OnEnable()
    {
        EventHandler.AddEventListener("Stage2Event", OnStage2Event);
        EventHandler.AddEventListener("BossDeadEvent", OnBossDeadEvent);
        EventHandler.AddEventListener<int, bool>("IceBallBecomeDrillEvent", OnIceBallBecomeDrillEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener("Stage2Event", OnStage2Event);
        EventHandler.RemoveEventListener("BossDeadEvent", OnBossDeadEvent);
        EventHandler.RemoveEventListener<int, bool>("IceBallBecomeDrillEvent", OnIceBallBecomeDrillEvent);
    }

    private void Update()
    {
        if (isStage2 && canRotate)
            transform.Rotate(Vector3.back, angularSpeed * Time.deltaTime, Space.World);
        //if (isStage2 && canRotate)
        //{
        //    if (ClockTimer > 2f)
        //        ClockRotate();
        //    else
        //        ClockTimer += Time.deltaTime;
        //}
    }

    private void ClockRotate()
    {
        var eulerAngle = transform.rotation.eulerAngles + new Vector3(0, 0, 30f);
        transform.rotation = Quaternion.Euler(eulerAngle);
        ClockTimer = 0;
    }

    private void OnStage2Event()
    {
        isStage2 = true;
        canRotate = true;
    }

    private void OnBossDeadEvent()
    {
        isStage2 = false;
        canRotate = false;
    }

    private void OnIceBallBecomeDrillEvent(int index, bool canRotate)
    {
        if (this.index == index)
        {
            this.canRotate = canRotate;
        }
    }
}
