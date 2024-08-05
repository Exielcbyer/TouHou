using UnityEngine;

public class Ground : MonoBehaviour
{
    // �ý�ɫ�͵��������ײ�������ǵ�Ļ���أ��ܹ�һ���̶��ϼ��ټ�����
    // ���������Ҫ����Rigibody2d��unity������ײ�����Ҫ����Ϊ��ײ���С��ͳһ��Physics2D������ļ�ⷶΧ����״�ϲ����
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
