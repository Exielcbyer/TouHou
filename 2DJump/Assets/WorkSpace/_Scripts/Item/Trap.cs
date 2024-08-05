using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage;
    public bool attackOver;
    private float attackIntervalTime;

    private void Update()
    {
        if (attackOver)
            IntervalTime();
    }

    private void IntervalTime()
    {
        if (attackIntervalTime > 1 && attackOver)
        {
            attackOver = false;
            attackIntervalTime = 0;
        }
        else
        {
            attackIntervalTime += Time.deltaTime;
        }
    }
}
