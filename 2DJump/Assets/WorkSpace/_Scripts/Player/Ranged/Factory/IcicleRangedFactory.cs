using UnityEngine;

public class IcicleRangedFactory : Singleton<IcicleRangedFactory>, RangedFactory
{
    public GameObject icicleRangedPrefab;

    public void CreatRanged(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject ranged = ObjectPool.Instance.GetObject(icicleRangedPrefab);
        ranged.GetComponent<BaseRanged>().Init(creatPos, direction, angle, target);
    }
}
