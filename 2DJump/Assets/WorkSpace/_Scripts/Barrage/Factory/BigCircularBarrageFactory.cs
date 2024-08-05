using UnityEngine;

public class BigCircularBarrageFactory : Singleton<BigCircularBarrageFactory>, BarrageFactory
{
    public GameObject bigCircularBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(bigCircularBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target);
    }
}
