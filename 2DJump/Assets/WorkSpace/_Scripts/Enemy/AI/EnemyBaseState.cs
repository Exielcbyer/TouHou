public abstract class EnemyBaseState
{
    protected Enemy currentEnemy;

    public abstract void EnterState(Enemy enemy);

    public abstract void OnUpdate();

    public abstract void OnExit();
}
