using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.animState = 0;
        currentEnemy.speed = 2f;
        currentEnemy.SwitchPoint();
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        if (!currentEnemy.anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))//ÅÐ¶Ïµ±Ç°¶¯»­×´Ì¬
        {
            currentEnemy.animState = 1;
            currentEnemy.Move();
        }
        if (Mathf.Abs(currentEnemy.transform.position.x - currentEnemy.targetPoint.position.x) <= currentEnemy.distance.x && Mathf.Abs(currentEnemy.transform.position.y - currentEnemy.targetPoint.position.y) <= currentEnemy.distance.y) 
        {
            currentEnemy.TransitionToState(currentEnemy.patrolState);
        }
        if (currentEnemy.attackTarget != null)
        {
            currentEnemy.TransitionToState(currentEnemy.attackState);
        }
    }
}
