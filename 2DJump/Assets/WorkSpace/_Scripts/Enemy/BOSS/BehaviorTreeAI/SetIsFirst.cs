using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SetIsFirst : Action
{
    public SharedBool isFirst;

    public override void OnStart()
    {
        EventHandler.TriggerEvent("Stage2Event");
        EventHandler.TriggerEvent("FilmShadeEvent", 100f);
        // �����״ν�����׶��ݳ�
        EventHandler.TriggerEvent<int>("TimeLinePlayEvent", 1);
        //isFirst.Value = false;
    }
}
