using UnityEngine;

public class StarBarrageFactory : Singleton<StarBarrageFactory>, BarrageFactory
{
    public GameObject starBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(starBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
