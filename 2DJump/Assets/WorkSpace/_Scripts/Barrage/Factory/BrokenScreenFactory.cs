using UnityEngine;

public class BrokenScreenFactory : Singleton<BrokenScreenFactory>, BarrageFactory
{
    public GameObject brokenScreenPrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(brokenScreenPrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction, angle, target);
    }
}
