using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    public float damage;
    public bool attackOver;

    private void OnEnable()
    {
        attackOver = false;
    }
}
