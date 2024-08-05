using UnityEngine;

public class StarBarrage : CircularBarrage
{
    private float randomScale;
    private int angularSpeed = 1;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        speed = 6f;
        if (index > 0)
            speed = Random.Range(3, 6);
        randomScale = Random.Range(0.35f, 0.6f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        this.direction = direction.x != 0 ? transform.right : -transform.up;
    }

    protected override void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (!isFrozen)
            transform.Rotate(0, 0, angularSpeed);

        SetFallSpeed();
    }
    #endregion

    public void SetStarSpeed()
    {
        speed *= 0.5f;
    }
}
