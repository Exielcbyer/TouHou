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
        isFrozen = false;
        isGuard = false;
        currentDuration = duration;
        frozenEffectObject.SetActive(false);
        transform.position = this.creatPos;
    }

    protected virtual void Update()
    {

    }

    // ����
    public virtual void Frozen()
    {
        isFrozen = true;
        frozenEffectObject.SetActive(true);
        StopCoroutine("IEnumeratorDestroy");
        currentDuration = frozenDuration;
        StartCoroutine("IEnumeratorDestroy");
    }

    // ����
    public virtual void Interact()
    { 
        
    }

    // ��ײ�����棬�ڵ�Ļλ��������Ч������أ�
    public virtual void CollGround()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(barrageParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // ��ײ����ң��ڵ�Ļλ��������Ч������أ���������ײЧ����һ����ͬ����˷�Ϊ��������
    public virtual void CollPlayer()
    {
        if (gameObject.activeSelf)
        {
            GameObject particle = ObjectPool.Instance.GetObject(barrageParticlesPrefab);
            particle.transform.position = transform.position;
        }
    }

    // ��ײ������
    public virtual void CollShield()
    {
        isGuard = true;
        CollGround();
    }

    // ��ײ���Լ�
    public virtual void CollEnemy()
    {

    }

    // ��ʱ���٣�����������Ч
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
