using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MasterSpark : BaseBarrage
{
    private GameObject masterSparkObject;
    private BoxCollider2D delayCollider;
    private ParticleSystem particle;
    private Transform boss;
    private Transform target => GameObject.Find("Player").transform;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        masterSparkObject = transform.GetChild(2).gameObject;
        delayCollider = transform.GetComponent<BoxCollider2D>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.boss = target;
        StartCoroutine(CannonRotate(angle));
        StartCoroutine(DelayStraightLine());
    }

    protected override void ReSet()
    {
        isFrozen = false;
        transform.position = this.creatPos;
        masterSparkObject.SetActive(false);
        delayCollider.enabled = false;
    }

    public override void Frozen()
    {

    }
    #endregion

    private IEnumerator DelayStraightLine()
    {
        yield return new WaitForSeconds(particle.main.duration / 2);
        masterSparkObject.SetActive(true);
        delayCollider.enabled = true;
    }

    private IEnumerator CannonRotate(float angle)
    {
        yield return new WaitForSeconds(2f);
        if (boss.position.x < target.position.x)
            transform.DORotate(new Vector3(0, 0, angle + 120f), 13f).SetEase(Ease.InOutSine);
        else
            transform.DORotate(new Vector3(0, 0, angle - 120f), 13f).SetEase(Ease.InOutSine);
    }
}
