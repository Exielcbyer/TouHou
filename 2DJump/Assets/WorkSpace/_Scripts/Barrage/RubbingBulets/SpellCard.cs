using UnityEngine;

public class SpellCard : MonoBehaviour
{
    public float cardEnergy;
    private float speed = 6f;
    private float angularSpeed = 60f;
    private Vector3 creatPos;
    private Vector3 direction;
    private float angle;
    private Transform target;

    public void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        this.creatPos = creatPos;
        transform.position = this.creatPos;
        this.direction = direction;
        this.target = target;
        this.cardEnergy = index;
    }

    private void Update()
    {
        direction = (this.target.position - transform.position).normalized;
        angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.rotation = transform.rotation * Quaternion.Euler(0, 0, angularSpeed);
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void CollPlayer()
    {
        ObjectPool.Instance.PushObject(gameObject);
    }
}
