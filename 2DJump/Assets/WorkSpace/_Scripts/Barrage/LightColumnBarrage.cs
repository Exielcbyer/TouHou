using UnityEngine;
using System.Collections;

public class LightColumnBarrage : BaseBarrage
{
    private Transform target;
    private int clockIndex;
    public LayerMask laserLayer;
    private LineRenderer laser;
    private GameObject endPointFX;
    private float intervalTime;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        laser = GetComponent<LineRenderer>();
        endPointFX = transform.GetChild(2).gameObject;
    }

    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.target = target;
        this.clockIndex = index;
        StartCoroutine(DelayLightColumn());
    }

    protected override void ReSet()
    {
        isFrozen = false;
        laser.enabled = false;
    }

    protected override void Update()
    {
        transform.rotation = target.rotation;
        transform.position = target.GetChild(0).position;
        if (laser.enabled)
            UpdateLaserEnd();
    }

    public override void CollGround()
    {

    }

    public override void CollShield()
    {

    }
    #endregion

    private IEnumerator DelayLightColumn()
    {
        yield return new WaitForSeconds(1.5f);
        laser.enabled = true;
    }

    private void UpdateLaserEnd()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y, -1);
        float rotationZ = transform.rotation.eulerAngles.z;
        rotationZ *= Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rotationZ), Mathf.Sin(rotationZ));
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction.normalized, Mathf.Infinity, laserLayer);
        // 由于射线长度会根据endPoint改变，所以不用碰撞体而是射线检测
        if (hit.transform.TryGetComponent(out PlayerCollision player))
        {
            if (!player.invincible)
                player.GetHit(damage);
        }
        if (hit.transform.TryGetComponent(out IceBallRanged iceBallRanged))
        {
            iceBallRanged.BecomeDrill(clockIndex);
        }
        if (hit.transform.TryGetComponent(out Shield shield))
        {
            if (intervalTime > 0.1f)
            {
                shield.GuardLaserOrLightColumn(damage);
                intervalTime = 0;
            }
            else
                intervalTime += Time.deltaTime;
        }

        laser.SetPosition(0, startPos);
        laser.SetPosition(1, hit.point);
        endPointFX.transform.position = hit.point;
    }
}
