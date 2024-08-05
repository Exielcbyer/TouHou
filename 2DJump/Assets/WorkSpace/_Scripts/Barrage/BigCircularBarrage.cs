using UnityEngine;

public class BigCircularBarrage : CircularBarrage
{
    private Transform target;
    protected bool fallOver;

    #region Override
    public override void Init(Vector3 creatPos, Vector3 direction, float angle = 0, Transform target = null, int index = 0)
    {
        base.Init(creatPos, direction, angle, target, index);
        this.target = target;
    }

    protected override void ReSet()
    {
        base.ReSet();
        fallOver = false;
    }

    public override void Interact()
    {
        if (isFrozen && fallOver)
        {
            direction = (this.target.position - transform.position).normalized;
            speed = 50f;
        }
    }

    public override void CollGround()
    {
        if (!isFrozen)
        {
            CreatCircularBarrages();
            base.CollGround();
        }
        else
        {
            fallOver = true;
            changingSpeed = false;
            speed = 0;
        }
    }

    public override void CollPlayer()
    {
        if (!isFrozen)
        {
            CreatCircularBarrages();
            base.CollPlayer();
        }
    }

    public override void CollEnemy()
    {
        if (isFrozen)
        {
            CreatCircularBarrages();
            base.CollGround();
        }
    }
    #endregion

    private void CreatCircularBarrages()
    {
        for (float i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                LittlePhoenixBarrageFactory.Instance.CreatBarrage(transform.position, new Vector3(i, j, 0));
            }
        }
    }

    public override void CollAttackArea()
    {
        
    }

    public override void CollVolleyRanged()
    {
        
    }
}
