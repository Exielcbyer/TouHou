using UnityEngine;

public class ShieldRangedFactory : Singleton<ShieldRangedFactory>, RangedFactory
{
    public GameObject shieldRangedPrefab;

    public void CreatRanged(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject ranged = ObjectPool.Instance.GetObject(shieldRangedPrefab);
        ranged.GetComponent<BaseRanged>().Init(creatPos, direction, angle, target, index);
    }
}
