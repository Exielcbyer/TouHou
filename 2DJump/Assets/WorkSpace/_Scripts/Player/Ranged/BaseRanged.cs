using System.Collections;
using UnityEngine;

public class BaseRanged : MonoBehaviour
{
    public GameObject rangedParticlesPrefab;

    public float damage;

    [SerializeField] protected float duration = 10f;
    protected Vector3 creatPos;
    protected Vector3 direction;
    protected float angle;
    [SerializeField] protected float speed = 20f;

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
        duration = 10f;
        transform.position = this.creatPos;
    }

    protected virtual void Update()
    {
        
    }

    // 碰撞到地面，在弹幕位置生成特效（对象池）
    public virtual void CollGround()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(rangedParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // 碰撞到敌人，在弹幕位置生成特效（对象池），两种碰撞效果不一定相同，因此分为两个函数
    public virtual void CollEnemy()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(rangedParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    protected virtual IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(duration);
        if (gameObject.activeSelf)
        {
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
