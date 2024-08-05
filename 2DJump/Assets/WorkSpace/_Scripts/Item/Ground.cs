using UnityEngine;

public class Ground : MonoBehaviour
{
    // 让角色和地面挂载碰撞检测而不是弹幕挂载，能够一定程度上减少计算量
    // 这里采用需要挂载Rigibody2d的unity物理碰撞检测主要是因为碰撞体大小不统一，Physics2D物理检测的检测范围和形状较不灵活
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BaseBarrage barrage))
        {
            barrage.CollGround();
        }
        if (collision.TryGetComponent(out BaseRanged ranged))
        {
            ranged.CollGround();
        }
    }
}
