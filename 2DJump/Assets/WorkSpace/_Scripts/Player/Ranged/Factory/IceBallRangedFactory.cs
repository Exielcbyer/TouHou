using UnityEngine;

public class IceBallRangedFactory : Singleton<IceBallRangedFactory>, RangedFactory
{
    public GameObject iceBallRangedPrefab;

    public void CreatRanged(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject ranged = ObjectPool.Instance.GetObject(iceBallRangedPrefab);
        ranged.GetComponent<BaseRanged>().Init(creatPos, direction, angle, target);
    }
}
