using UnityEngine;

public class LockArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameManager.Instance.enemyList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameManager.Instance.enemyList.Remove(collision.gameObject);
        }
    }
}
