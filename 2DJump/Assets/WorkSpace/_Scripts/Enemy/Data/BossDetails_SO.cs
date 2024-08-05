using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossDetails_SO", menuName = "Enemy/BossDetails")]
public class BossDetails_SO : ScriptableObject
{
    public float heath = 0;
    public float maxHeath = 100;
    public bool isDead;
}
