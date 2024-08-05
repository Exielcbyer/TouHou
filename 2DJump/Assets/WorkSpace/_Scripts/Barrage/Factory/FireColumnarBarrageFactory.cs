using UnityEngine;


public class FireColumnarBarrageFactory : Singleton<FireColumnarBarrageFactory>, BarrageFactory
{
    public GameObject fireColumnarPrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(fireColumnarPrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle);
    }
}