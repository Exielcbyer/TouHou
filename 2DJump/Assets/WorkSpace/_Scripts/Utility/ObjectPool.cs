using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    public GameObject pool;//��Ϊ���ɵĲ�ͬ����Ķ�����ǵĸ�����

    /// <summary>
    /// �Ӷ���ض�����ȡ�����岢���ɣ���ʾ��
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public GameObject GetObject(GameObject prefab)
    {
        GameObject gameObject;
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            gameObject = GameObject.Instantiate(prefab);
            PushObject(gameObject);
            if (pool == null)
            {
                pool = new GameObject("Pool");
                pool.transform.SetParent(transform);
            }
            //���������ĸ��ӹ�ϵ
            GameObject childPool = GameObject.Find(prefab.name + "Pool");//ÿ������Ķ���ظ�����
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform);
            }
            gameObject.transform.SetParent(childPool.transform);
        }
        gameObject = objectPool[prefab.name].Dequeue();//ȡ������
        gameObject.SetActive(true);
        return gameObject;
    }

    /// <summary>
    /// ���٣����أ����岢ѹ�����ض���
    /// </summary>
    /// <param name="prefab"></param>
    public void PushObject(GameObject prefab)
    {
        string name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objectPool.ContainsKey(name))
            objectPool.Add(name, new Queue<GameObject>());
        objectPool[name].Enqueue(prefab);//ѹ������
        prefab.SetActive(false);
    }

    /// <summary>
    /// ��ն����
    /// </summary>
    public void ClearObjectPool()
    {
        if (pool != null)
        {
            objectPool.Clear();
            Destroy(pool);
        }
    }
}