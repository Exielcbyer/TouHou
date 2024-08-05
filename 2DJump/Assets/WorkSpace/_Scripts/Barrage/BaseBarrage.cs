using System.Collections;
using UnityEngine;

public class BaseBarrage : MonoBehaviour
{
    public GameObject barrageParticlesPrefab;
    public GameObject frozenEffectObject;

    public bool isFrozen;
    public bool isGuard;

    public float damage = 1f;
    [SerializeField] protected float duration = 5f;
    [SerializeField] protected float frozenDuration = 20f;
    protected float currentDuration;
    protected Vector3 creatPos;
    protected Vector3 direction;
    protected float angle;

    protected virtual void Awake()
    {
        currentDuration = duration;
    }

    // 初始化，在从对象池取出前执行（OnEnable）
    public virtual void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        StartCoroutine("IEnumeratorDestroy");
        this.creatPos = creatPos;
        transform.position = this.creatPos;
        this.direction = direction;
        this.angle = angle;
        transform.rotation = Quaternion.Euler(0, 0, this.angle);
    }

    // 重置期间修改的属性，放回对象池前执行（OnDisable）
    protected virtual void ReSet()
    {
        isFrozen = false;
        isGuard = false;
        currentDuration = duration;
        frozenEffectObject.SetActive(false);
        transform.position = this.creatPos;
    }

    protected virtual void Update()
    {

    }

    // 冻结
    public virtual void Frozen()
    {
        isFrozen = true;
        frozenEffectObject.SetActive(true);
        StopCoroutine("IEnumeratorDestroy");
        currentDuration = frozenDuration;
        StartCoroutine("IEnumeratorDestroy");
    }

    // 互动
    public virtual void Interact()
    { 
        
    }

    // 碰撞到地面，在弹幕位置生成特效（对象池）
    public virtual void CollGround()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(barrageParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // 碰撞到玩家，在弹幕位置生成特效（对象池），两种碰撞效果不一定相同，因此分为两个函数
    public virtual void CollPlayer()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(barrageParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // 碰撞到护盾
    public virtual void CollShield()
    {
        isGuard = true;
        CollGround();
    }

    // 碰撞到自己
    public virtual void CollEnemy()
    {

    }

    // 超时销毁，不用生成特效
    protected virtual IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(currentDuration);
        if (gameObject.activeSelf)
        {
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
