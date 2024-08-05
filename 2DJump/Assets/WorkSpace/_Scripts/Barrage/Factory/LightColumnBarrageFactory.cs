using UnityEngine;

public class LightColumnBarrageFactory : Singleton<LightColumnBarrageFactory>, BarrageFactory
{
    public GameObject lightColumnBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(lightColumnBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
