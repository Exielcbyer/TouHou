using UnityEngine;

public class CircularBarrageFactory : Singleton<CircularBarrageFactory>, BarrageFactory
{
    public GameObject circularBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(circularBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
