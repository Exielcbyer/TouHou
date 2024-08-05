using UnityEngine;

public class StraightLineFactory : Singleton<StraightLineFactory>, BarrageFactory
{
    public GameObject straightLinePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(straightLinePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target, index);
    }
}
