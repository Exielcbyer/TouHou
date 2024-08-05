using UnityEngine;

public class ShieldRanged : BaseRanged
{
    public bool isRevert;

    private float acceleration = 30f;
    private float rotateSpeed = 5f;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target);
        transform.rotation = Quaternion.Euler(70f, this.angle, 0);
        transform.localScale = new Vector3(index, index, 1);
        speed = 20f;
        isRevert = false;
    }

    protected override void Update()
    {
        transform.Rotate(0, 0, rotateSpeed);
        transform.position += direction * speed * Time.deltaTime;
        speed -= acceleration * Time.deltaTime;

        if (speed < 0)
        {
            isRevert = true;
            acceleration = 30f;
        }
        else if (speed < 2f)
            acceleration = 2f;
    }

    public override void CollGround()
    {

    }

    public override void CollEnemy()
    {
        if (speed > 8f)
            speed = 8f;
    }
    #endregion
}
