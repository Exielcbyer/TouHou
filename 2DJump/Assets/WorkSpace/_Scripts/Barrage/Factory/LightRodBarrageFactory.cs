using UnityEngine;

public class LightRodBarrageFactory : Singleton<LightRodBarrageFactory>, BarrageFactory
{
    public GameObject lightRodBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(lightRodBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
        barrage.GetComponent<LightRodBarrage>().SetMoveClamp(index);
    }
}
