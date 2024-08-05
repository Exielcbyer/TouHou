using System.Collections;
using UnityEngine;

public class StraightLine : BaseBarrage
{
    private int energy = 5;
    private Transform target;

    public GameObject spellCardsPrefab;
    private GameObject straightLineObject;
    private BoxCollider2D delayCollider;
    private ParticleSystem particle;

    #region Override
    protected override void Awake()
    {
        base.Awake();
        straightLineObject = transform.GetChild(1).gameObject;
        delayCollider = transform.GetComponent<BoxCollider2D>();
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.target = target;
        StartCoroutine(DelayStraightLine());
    }

    protected override void ReSet()
    {
        if (isFrozen)
        {
            CreatSpellCards();
        }
        isFrozen = false;
        isGuard = false;
        frozenEffectObject.SetActive(false);
        transform.position = this.creatPos;
        straightLineObject.SetActive(false);
        delayCollider.enabled = false;
    }

    public override void Frozen()
    {
        base.Frozen();
        straightLineObject.SetActive(false);
        delayCollider.enabled = false;
    }
    #endregion

    private void CreatSpellCards()
    {
        for (int i = 0; i < 3; i++)
        {
            SpellCardFactory.Instance.CreatBarrage(target.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 1f), 0), Vector3.zero, 0, target, energy);
        }
    }

    private IEnumerator DelayStraightLine()
    {
        yield return new WaitForSeconds(particle.main.duration);
        straightLineObject.SetActive(true);
        delayCollider.enabled = true;
    }
}
