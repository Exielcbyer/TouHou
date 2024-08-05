using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StageConditional : Conditional
{
    public BossDetails_SO bossDetails;
    public SharedBool isStage2;

    public override TaskStatus OnUpdate()
    {
        if (bossDetails.heath <= bossDetails.maxHeath / 2 && isStage2.Value)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
