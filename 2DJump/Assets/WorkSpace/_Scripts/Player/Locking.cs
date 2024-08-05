using UnityEngine;

public class Locking : MonoBehaviour
{
    private Transform target;

    private void OnDisable()
    {
        target = null;
    }

    private void Update()
    {
        if (target != null)
            transform.position = target.position;
    }

    public void LockTarget(Transform target)
    {
        this.target = target;
    }
}
