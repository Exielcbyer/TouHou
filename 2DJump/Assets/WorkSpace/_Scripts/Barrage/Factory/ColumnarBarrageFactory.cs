using UnityEngine;

public class ColumnarBarrageFactory : Singleton<ColumnarBarrageFactory>, BarrageFactory
{
    public GameObject columnarBarragePrefab;

    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        GameObject barrage = ObjectPool.Instance.GetObject(columnarBarragePrefab);
        barrage.GetComponent<BaseBarrage>().Init(creatPos, direction);
    }
}
