using UnityEngine;

public class DiamondBarrageFactory : Singleton<DiamondBarrageFactory>, BarrageFactory
{
    public GameObject diamondBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(diamondBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target);
    }
}
