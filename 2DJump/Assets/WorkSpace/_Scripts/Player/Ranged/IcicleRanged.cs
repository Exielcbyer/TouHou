using UnityEngine;

public class IcicleRanged : BaseRanged
{
    #region Override
    protected override void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    public override void CollGround()
    {
        base.CollGround();
        ReSet();
        ObjectPool.Instance.PushObject(gameObject);
    }

    public override void CollEnemy()
    {
        base.CollEnemy();
        ReSet();
        ObjectPool.Instance.PushObject(gameObject);
    }
    #endregion
}
