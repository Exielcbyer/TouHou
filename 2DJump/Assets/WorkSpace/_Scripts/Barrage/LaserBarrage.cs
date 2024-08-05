using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LaserBarrage : BaseBarrage
{
    private Transform target;
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
        if (index == -30)
            StartCoroutine(LaserRotate(index, true));
        else
            StartCoroutine(LaserRotate(index, false));
        this.target = target;
        StartCoroutine(DelayLaser());
    }

    protected override void ReSet()
    {
        isFrozen = false;
        laser.enabled = false;
        transform.position = this.creatPos;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void Update()
    {
        if (target != null)
            transform.position = target.position;
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

    private IEnumerator DelayLaser()
    {
        yield return new WaitForSeconds(1.5f);
        laser.enabled = true;
    }

    private IEnumerator LaserRotate(float endAngle, bool isRepeat)
    {
        if (isRepeat)
        {
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle - 30), 1.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle - 120), 1.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle), 1.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle - 140), 1.5f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle + 20), 2f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle - 120), 2f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1.5f);
            transform.DORotate(new Vector3(0, 0, endAngle), 2f).SetEase(Ease.InOutQuad);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            transform.DORotate(new Vector3(0, 0, endAngle), 5f).SetEase(Ease.InOutQuad);
        }
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
        if (hit.transform.TryGetComponent(out SquareBarrage squareBarrage))
        {
            squareBarrage.CollPlayer();
        }
        if (hit.transform.TryGetComponent(out Shield shield))
        {
            transform.DOPause();
            var dir = (shield.transform.position - transform.position).normalized;
            var angle = Vector3.SignedAngle(dir, Vector3.right, Vector3.back);
            var rotate = Quaternion.Euler(0, 0, angle);
            transform.rotation = rotate;
            if (intervalTime > 0.1f)
            {
                shield.GuardLaserOrLightColumn(damage);
                shield.GuardLaser();
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
