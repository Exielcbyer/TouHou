using UnityEngine;

public class ChaosBarrage : CircularBarrage
{
    private Transform target;
    private float delayTime = 0;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.target = target;
        speed = 15f;
    }

    protected override void ReSet()
    {
        base.ReSet();
        delayTime = 0;
    }

    protected override void Update()
    {
        if (delayTime > 2f && delayTime < 4f) // »ºÂý×ªÏò
        {
            direction = (target.position - transform.position).normalized;
            angle = Vector3.SignedAngle(direction, Vector3.right, Vector3.back);
            var rotate =  Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime);
        }
        else if (delayTime > 4f)
            transform.position += transform.right * speed * Time.deltaTime;

        delayTime += Time.deltaTime;

        SetFallSpeed();
    }

    public override void CollEnemy()
    {
        if (isFrozen)
        {
            CollGround();
        }
    }
    #endregion
}
