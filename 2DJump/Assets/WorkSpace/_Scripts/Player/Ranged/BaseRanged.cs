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

    // ��ʼ�����ڴӶ����ȡ��ǰִ�У�OnEnable��
    public virtual void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        StartCoroutine("IEnumeratorDestroy");
        this.creatPos = creatPos;
        transform.position = this.creatPos;
        this.direction = direction;
        this.angle = angle;
        transform.rotation = Quaternion.Euler(0, 0, this.angle);
    }

    // �����ڼ��޸ĵ����ԣ��Żض����ǰִ�У�OnDisable��
    protected virtual void ReSet()
    {
        duration = 10f;
        transform.position = this.creatPos;
    }

    protected virtual void Update()
    {
        
    }

    // ��ײ�����棬�ڵ�Ļλ��������Ч������أ�
    public virtual void CollGround()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(rangedParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // ��ײ�����ˣ��ڵ�Ļλ��������Ч������أ���������ײЧ����һ����ͬ����˷�Ϊ��������
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
