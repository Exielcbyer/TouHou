using UnityEngine;

public class VolleyRangedFactory : Singleton<VolleyRangedFactory>, RangedFactory
{
    public GameObject volleyRangedPrefab;

    public void CreatRanged(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject ranged = ObjectPool.Instance.GetObject(volleyRangedPrefab);
        ranged.GetComponent<BaseRanged>().Init(creatPos, direction, angle, target);
    }
}
