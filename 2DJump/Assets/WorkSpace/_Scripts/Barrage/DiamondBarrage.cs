using UnityEngine;

public class DiamondBarrage : BaseBarrage
{
    private Transform target;
    protected float speed = 10f;
    protected float angularSpeed = 60f;
    protected float randomAngle;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle, Transform target, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.target = target;
        randomAngle = Random.Range(-30, 90);
    }

    protected override void ReSet()
    {
        base.ReSet();
        speed = 10f;
    }

    protected override void Update()
    {
        if (!isFrozen)
        {
            direction = (this.target.position - transform.position).normalized;
            float angle = randomAngle - Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
            transform.rotation = transform.rotation * Quaternion.Euler(0, 0, angularSpeed);
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    public override void Frozen()
    {
        base.Frozen();
        speed = 0;
    }

    public override void Interact()
    {
        this.target = GameObject.Find("Marisa").transform;
        speed = 10f;
        isFrozen = false;//换一个触发点
    }

    public override void CollGround()
    {
        //base.CollGround();
        //ReSet();
        //ObjectPool.Instance.PushObject(gameObject);
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
}
