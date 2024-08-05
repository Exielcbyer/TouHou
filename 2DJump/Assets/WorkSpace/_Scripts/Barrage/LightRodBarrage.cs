using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRodBarrage : BaseBarrage
{ 
    private float acceleration = 80f;
    private float moveClamp = 8f;
    private float fallClamp = -30f;
    private bool changingSpeed;
    private float currentHorizontalSpeed;
    private float currentVerticalSpeed;
    private int angularSpeed = 1;
    private Vector3 eulerAngle;
    private Transform lightColumnPos;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        lightColumnPos = transform.GetChild(0);
    }

    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        currentHorizontalSpeed = 10;
        currentVerticalSpeed = 10;
        moveClamp = 8f;
        changingSpeed = true;
    }

    protected override void Update()
    {
        transform.position += new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;

        SetFallSpeed();
    }

    public override void CollGround()
    {
        base.CollGround();
        changingSpeed = false;
        currentHorizontalSpeed = 0;
        currentVerticalSpeed = 0;
        AngleCorrection();
        LightColumnBarrageFactory.Instance.CreatBarrage(lightColumnPos.position, direction, 0, transform);
    }

    public override void CollPlayer()
    {

    }

    protected override IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(currentDuration);
        ReSet();
        ObjectPool.Instance.PushObject(gameObject);   
    }
    #endregion


    private void SetFallSpeed()
    {
        if (changingSpeed)
        {
            currentHorizontalSpeed += direction.x * acceleration * Time.deltaTime;

            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed, -moveClamp, moveClamp);

            currentVerticalSpeed -= acceleration * Time.deltaTime;

            if (currentVerticalSpeed < fallClamp)
                currentVerticalSpeed = fallClamp;

            transform.Rotate(0, 0, angularSpeed);
        }
    }

    private void AngleCorrection()
    {
        eulerAngle = transform.rotation.eulerAngles;
        if (eulerAngle.z < 0)
            eulerAngle = -eulerAngle;
        else if (eulerAngle.z > 180)
        {
            eulerAngle -= new Vector3(0, 0, 180f);
        }
        if (eulerAngle.z < 30f)
        {
            eulerAngle += new Vector3(0, 0, 30f);
        }
        else if (eulerAngle.z > 150f)
        {
            eulerAngle -= new Vector3(0, 0, 30f);
        }
        transform.rotation = Quaternion.Euler(eulerAngle);
    }

    public void SetMoveClamp(int Count)
    {
        currentHorizontalSpeed += Count;
        currentVerticalSpeed += Count * 5;
        moveClamp *= Count;
    }
}
