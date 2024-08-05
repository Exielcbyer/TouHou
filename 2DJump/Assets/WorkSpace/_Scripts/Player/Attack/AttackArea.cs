using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public float damage;
    public bool isMulti;// ÊÇ·ñÊÇ¶à¶Î¹¥»÷
    private float attackIntervalTime;// ¶à¶Î¹¥»÷¼ä¸ô
    public bool attackOver;

    Vector2 size = new Vector2(1, 1.4f);
    Collider2D[] colliders = new Collider2D[100];

    public PlayerDetails_SO playerDetails;

    private void OnEnable()
    {
        attackOver = false;
    }

    private void Update()
    {
        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            if (coll.gameObject.TryGetComponent(out CircularBarrage circularBarrage))
            {
                playerDetails.energy += 0.25f;
                circularBarrage.CollAttackArea();
            }
        }

        if (isMulti)
            MultiIntervalTime();
    }

    private void MultiIntervalTime()
    {
        if (attackIntervalTime > 0.15f && attackOver)
        {
            attackOver = false;
            attackIntervalTime = 0;
        }
        else
        {
            attackIntervalTime += Time.deltaTime;
        }
    }
}
