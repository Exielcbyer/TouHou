using System.Collections;
using UnityEngine;

public class SquareBarrage : BaseBarrage
{
    public GameObject landmineParticlesPrefab;
    public GameObject landmineFrozenParticlesPrefab;

    protected float speed = 10f;
    protected float acceleration = 80f;
    protected float moveClamp = 8f;
    protected float fallClamp = -30f;

    #region Override

    protected override void ReSet()
    {
        base.ReSet();
        speed = 10f;
        moveClamp = 8f;
    }

    protected override void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public override void CollGround()
    {
        base.CollGround();
        speed = 0;
    }

    public override void CollPlayer()
    {
        if (!isFrozen)
        {
            if (gameObject.activeSelf)
            {
                GameObject particle = ObjectPool.Instance.GetObject(landmineParticlesPrefab);
                particle.transform.position = transform.position;
            }
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
        else
        {
            if (gameObject.activeSelf)
            {
                GameObject particle = ObjectPool.Instance.GetObject(landmineFrozenParticlesPrefab);
                particle.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            }
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    protected override IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(currentDuration);
        if (speed == 0)
            CollPlayer();
        else
        {
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
    #endregion
}
