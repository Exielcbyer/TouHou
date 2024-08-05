using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    public GameObject pool;//作为生成的不同物体的对象池们的父物体

    /// <summary>
    /// 从对象池队列中取出物体并生成（显示）
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
            //设置物体间的父子关系
            GameObject childPool = GameObject.Find(prefab.name + "Pool");//每种物体的对象池父物体
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform);
            }
            gameObject.transform.SetParent(childPool.transform);
        }
        gameObject = objectPool[prefab.name].Dequeue();//取出物体
        gameObject.SetActive(true);
        return gameObject;
    }

    /// <summary>
    /// 销毁（隐藏）物体并压入对象池队列
    /// </summary>
    /// <param name="prefab"></param>
    public void PushObject(GameObject prefab)
    {
        string name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objectPool.ContainsKey(name))
            objectPool.Add(name, new Queue<GameObject>());
        objectPool[name].Enqueue(prefab);//压入物体
        prefab.SetActive(false);
    }

    /// <summary>
    /// 清空对象池
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