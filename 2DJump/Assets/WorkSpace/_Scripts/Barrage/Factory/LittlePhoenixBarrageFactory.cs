using UnityEngine;

public class LittlePhoenixBarrageFactory : Singleton<LittlePhoenixBarrageFactory>, BarrageFactory
{
    public GameObject littlePhoenixeBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(littlePhoenixeBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction);
    }
}
