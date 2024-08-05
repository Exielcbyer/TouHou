using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_SO", menuName = "Enemy/EnemyDetailsList")]
public class EnemyDetails_SO : ScriptableObject
{
    public List<EnemyDetatils> enemyDetatilsList;
}

[System.Serializable]
public class EnemyDetatils
{
    public int enemyID;
    public float heath = 0;
    public float maxHeath = 100;
    public bool isDead;
    public EnemyType enemyType;
}
