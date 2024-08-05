using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SetIsStage2 : Action
{
    public SharedBool isStage2;

    public override void OnStart()
    {
        // ����������׶��¼�
        EventHandler.TriggerEvent("Stage2Event");
        isStage2.Value = true;
    }
}
