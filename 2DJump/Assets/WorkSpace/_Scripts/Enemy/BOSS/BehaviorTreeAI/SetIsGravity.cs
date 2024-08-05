using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class SetIsGravity : Action
{
    private BOSS BOSS;
    [SerializeField] private bool isGravity;

    public override void OnAwake()
    {
        BOSS = transform.GetComponent<BOSS>();
    }

    public override void OnStart()
    {
        BOSS.isGravity = this.isGravity;
    }
}
