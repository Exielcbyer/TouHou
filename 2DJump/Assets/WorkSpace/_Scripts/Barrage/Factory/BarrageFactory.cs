using UnityEngine;

//���ù�������ģʽ
public interface BarrageFactory
{
    public void CreatBarrage(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0);
}

