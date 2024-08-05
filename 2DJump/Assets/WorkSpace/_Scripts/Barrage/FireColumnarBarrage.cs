using System.Collections;
using UnityEngine;

public class FireColumnarBarrage : BaseBarrage
{
    public GameObject fireColumnarObject;
    private BoxCollider2D delayCollider;
    private ParticleSystem particle;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        fireColumnarObject = transform.GetChild(1).gameObject;
        delayCollider = transform.GetComponent<BoxCollider2D>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        StartCoroutine(DelayStraightLine());
    }

    protected override void ReSet()
    {
        isFrozen = false;
        transform.position = this.creatPos;
        fireColumnarObject.SetActive(false);
        delayCollider.enabled = false;
    }

    public override void Frozen()
    {
        base.Frozen();
    }
    #endregion

    private IEnumerator DelayStraightLine()
    {
        yield return new WaitForSeconds(particle.main.duration / 2);
        fireColumnarObject.SetActive(true);
        delayCollider.enabled = true;
    }
}
