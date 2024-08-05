using System;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]private float shieldCharge = 0;
    private float maxCharge = 30;

    Vector2 size = new Vector2(0.5f, 1f);// 
    Collider2D[] colliders = new Collider2D[100];

    public bool isOverheat;
    public bool isGuarding;
    public bool isGuardingLaser;

    private void OnEnable()
    {
        shieldCharge = 0;
    }

    private void OnDisable()
    {
        isGuarding = false;
        isGuardingLaser = false;
        if (shieldCharge >= maxCharge)
        {
            isOverheat = true;
            EventHandler.TriggerEvent<bool>("UpdateConvoluteShieldEvent", isOverheat);
        }
    }

    private void Update()
    {
        if (shieldCharge < maxCharge)
        {
            Array.Clear(colliders, 0, colliders.Length);
            Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

            foreach (var coll in colliders)
            {
                if (coll == null)
                    break;
                if (coll.gameObject.TryGetComponent(out BaseBarrage barrage))
                {
                    if (!barrage.isGuard)
                    {
                        isGuarding = true;
                        shieldCharge += barrage.damage;
                        barrage.CollShield();
                    }
                }
            }
        }
    }

    public void GuardLaserOrLightColumn(float damage)
    {
        if (shieldCharge < maxCharge)
            shieldCharge += damage;
        isGuarding = true;
    }

    public void GuardLaser()
    {
        isGuardingLaser = true;
    }
}
