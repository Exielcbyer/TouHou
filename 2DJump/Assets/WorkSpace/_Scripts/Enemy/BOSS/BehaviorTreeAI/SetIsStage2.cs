using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SetIsStage2 : Action
{
    public SharedBool isStage2;

    public override void OnStart()
    {
        // 触发进入二阶段事件
        EventHandler.TriggerEvent("Stage2Event");
        isStage2.Value = true;
    }
}
