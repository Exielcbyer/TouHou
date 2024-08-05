using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!enemy.attackTarget && !enemy.isDead)
            {
                enemy.attackTarget = collision.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.attackTarget = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!enemy.isDead)
            {
                //StartCoroutine(OnAlarm());
            }
        }
    }
}
