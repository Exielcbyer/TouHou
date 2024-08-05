using System.Collections;
using UnityEngine;

public class CircularBarrage : BaseBarrage
{
    public Color[] colors = new Color[5];
    protected SpriteRenderer spriteRenderer;

    protected float speed = 10f;
    protected float acceleration = 80f;
    protected float fallClamp = -30f;
    protected bool changingSpeed;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        spriteRenderer.color = colors[Random.Range(0, colors.Length)];

        speed = 10f;
        if (index == -1)
            speed = 6f;
        else if (index > 0)
            speed = Random.Range(5, 8);

        this.direction = transform.right;
    }

    protected override void ReSet()
    {
        base.ReSet();
        speed = 10f;
        changingSpeed = false;
    }

    protected override void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        SetFallSpeed();
    }

    public override void Frozen()
    {
        base.Frozen();
        speed = 0;
        StartCoroutine(Fall());
    }

    public override void CollGround()
    {
        base.CollGround();
        ReSet();
        ObjectPool.Instance.PushObject(gameObject);
    }

    public override void CollPlayer()
    {
        if (!isFrozen)
        {
            base.CollPlayer();
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
    #endregion

    protected IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.8f);
        direction = Vector3.up;
        changingSpeed = true;
    }

    protected virtual void SetFallSpeed()
    {
        if (changingSpeed)
        {
            speed -= acceleration * Time.deltaTime;

            if (speed < fallClamp)
                speed = fallClamp;
        }
    }

    public virtual void CollAttackArea()
    {
        CollGround();
    }

    public virtual void CollVolleyRanged()
    {
        CollGround();
    }
}
