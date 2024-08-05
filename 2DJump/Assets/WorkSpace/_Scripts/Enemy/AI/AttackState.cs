using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float time;

    public override void EnterState(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.animState = 2;
        currentEnemy.speed = 3f;
        currentEnemy.targetPoint = currentEnemy.attackTarget;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if (currentEnemy.attackTarget == null)
        {
            currentEnemy.TransitionToState(currentEnemy.patrolState);
        }

        if (currentEnemy.attackTarget != null && currentEnemy.attackTarget.CompareTag("Player"))
        {
            currentEnemy.AttackAtion();
            currentEnemy.SkillAction();
        }

        if (Mathf.Abs(currentEnemy.transform.position.x - currentEnemy.targetPoint.position.x) < 8f && Mathf.Abs(currentEnemy.transform.position.y - currentEnemy.targetPoint.position.y) < 8f)
        {
            if (time > 0.5f)
            {
                currentEnemy.RandomPoint();
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
        currentEnemy.Move();
    }
}