using System;
using UnityEngine;

public class VolleyRanged : BaseRanged
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite volleyImage;
    [SerializeField] private Sprite stabImage;

    Collider2D[] colliders = new Collider2D[20];
    public bool isColl;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<Transform>("StabEnemy", StabEnemy);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<Transform>("StabEnemy", StabEnemy);
    }

    #region Override
    protected override void ReSet()
    {
        base.ReSet();
        speed = 20f;
        isColl = false;
        spriteRenderer.sprite = volleyImage;
    }

    protected override void Update()
    {
        if (!isColl)
        {
            Array.Clear(colliders, 0, colliders.Length);
            Physics2D.OverlapCircleNonAlloc(transform.position, 0.32f, colliders);
            foreach (var coll in colliders)
            {
                if (coll == null)
                    break;
                if (coll.gameObject.TryGetComponent(out CircularBarrage barrage))
                {
                    barrage.CollVolleyRanged();
                    speed = 0;
                    isColl = true;
                }
            }
        }

        transform.position += transform.right * speed * Time.deltaTime;
    }

    public override void CollGround()
    {

    }

    public override void CollEnemy()
    {
        base.CollEnemy();
        ReSet();
        ObjectPool.Instance.PushObject(gameObject);
    }
    #endregion

    public void StabEnemy(Transform target)
    {
        if (isColl)
        {
            if (target)
            {
                speed = 30f;
                direction = (target.position - transform.position).normalized;
                transform.right = direction;
                spriteRenderer.sprite = stabImage;
            }
            else
            {
                base.CollGround();
                ReSet();
                ObjectPool.Instance.PushObject(gameObject);
            }
        }
    }
}
