using UnityEngine;

public class SquareBarrageFactory : Singleton<SquareBarrageFactory>, BarrageFactory
{
    public GameObject squareBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(squareBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
