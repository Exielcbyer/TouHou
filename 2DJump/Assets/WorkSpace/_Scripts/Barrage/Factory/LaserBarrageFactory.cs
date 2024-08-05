using UnityEngine;

public class LaserBarrageFactory : Singleton<LaserBarrageFactory>, BarrageFactory
{
    public GameObject laserBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(laserBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
