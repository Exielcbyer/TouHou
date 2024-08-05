using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FirstChangeStageConditional : Conditional
{
    public BossDetails_SO bossDetails;
    public SharedBool isFirst;

    public override TaskStatus OnUpdate()
    {
        if (bossDetails.heath <= bossDetails.maxHeath / 2 && isFirst.Value) 
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
