using UnityEngine;

public class LittlePhoenixBarrage : CircularBarrage
{
    protected float currentHorizontalSpeed;
    protected float currentVerticalSpeed = 20f;
    protected float moveClamp = 8f;
    protected bool changingParabolaSpeed;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.direction = direction;
        currentVerticalSpeed = direction.y > 0 ? 20f : 0;
        changingParabolaSpeed = true;
    }

    protected override void ReSet()
    {
        base.ReSet();
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = direction.y > 0 ? 20f : 0;
        changingSpeed = false;
        changingParabolaSpeed = false;
    }

    protected override void Update()
    {
        transform.position += new Vector3(currentHorizontalSpeed, currentVerticalSpeed) * Time.deltaTime;


        SetParabolaFallSpeed();
        SetFallSpeed();
    }

    public override void Frozen()
    {
        base.Frozen();
        changingParabolaSpeed = false;
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
    }

    protected override void SetFallSpeed()
    {
        if (changingSpeed)
        {
            currentVerticalSpeed -= acceleration * Time.deltaTime;

            if (currentVerticalSpeed < fallClamp)
                currentVerticalSpeed = fallClamp;
        }
    }

    public override void CollEnemy()
    {
        CollGround();
    }
    #endregion

    private void SetParabolaFallSpeed()
    {
        if (changingParabolaSpeed)
        {
            currentHorizontalSpeed += direction.x * acceleration * Time.deltaTime;

            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

            currentVerticalSpeed -= acceleration * Time.deltaTime;

            if (currentVerticalSpeed < fallClamp)
                currentVerticalSpeed = fallClamp;
        }
    }
}
