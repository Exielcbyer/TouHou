using System;
using System.Collections;
using UnityEngine;

public class IceBallRanged : BaseRanged
{
    private float currentHorizontalSpeed = 20f;
    private float currentVerticalSpeed = 0;
    private float acceleration = 40f;
    private float fallClamp = -30f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite iceBallImage;
    [SerializeField] private Sprite drillImage;
    public bool becomeDrill;
    private bool changingSpeed;

    Vector2 size = new Vector2(1, 1.4f);
    Collider2D[] colliders = new Collider2D[100];

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target);
        transform.right = direction;
    }

    protected override void ReSet()
    {
        base.ReSet();
        currentHorizontalSpeed = 20f;
        currentVerticalSpeed = 0;
        becomeDrill = false;
        changingSpeed = false;
        spriteRenderer.sprite = iceBallImage;
    }

    protected override void Update()
    {
        transform.position += new Vector3(currentHorizontalSpeed * direction.x, currentVerticalSpeed) * Time.deltaTime;

        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapBoxNonAlloc(transform.position, size, 0, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            if (coll.gameObject.TryGetComponent(out BigCircularBarrage bigCircularBarrage))
            {
                bigCircularBarrage.CollGround();
                CollGround();
            }
        }

        SetFallSpeed();
    }

    public override void CollGround()
    {
        if (!becomeDrill)
        {
            base.CollGround();
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
        else
        {
            changingSpeed = false;
            currentHorizontalSpeed = 0;
            currentVerticalSpeed = 0;
        }
    }

    public override void CollEnemy()
    {
        if (!becomeDrill)
        {
            base.CollEnemy();
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
    #endregion

    private void SetFallSpeed()
    {
        if (becomeDrill && changingSpeed) 
        {
            if (currentHorizontalSpeed > 0)
                currentHorizontalSpeed -= acceleration * Time.deltaTime;
            else
                currentHorizontalSpeed = 0;
            
            currentVerticalSpeed -= acceleration * Time.deltaTime;

            if (currentVerticalSpeed < fallClamp)
                currentVerticalSpeed = fallClamp;
        }
    }

    public void CollPlayer()
    {
        if (becomeDrill)
        {
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    public void BecomeDrill(int clockIndex)
    {
        if (!becomeDrill)
        {
            StartCoroutine(BecomeingDrill(clockIndex));
            becomeDrill = true;
        }
    }

    private IEnumerator BecomeingDrill(int clockIndex)
    {
        EventHandler.TriggerEvent<int, bool>("IceBallBecomeDrillEvent", clockIndex, false);
        currentHorizontalSpeed = 0.5f;
        yield return new WaitForSeconds(1.5f);
        currentHorizontalSpeed = 30f;
        spriteRenderer.sprite = drillImage;
        changingSpeed = true;
        EventHandler.TriggerEvent<int, bool>("IceBallBecomeDrillEvent", clockIndex, true);
    }
}
