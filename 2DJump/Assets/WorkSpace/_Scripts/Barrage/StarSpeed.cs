using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpeed : MonoBehaviour
{
    private StarBarrage starBarrage;
    private bool hasSetSpeed;

    private void Awake()
    {
        starBarrage = GetComponentInParent<StarBarrage>();
    }

    private void OnEnable()
    {
        hasSetSpeed = false;
    }

    public void SetStarSpeed()
    {
        if (!hasSetSpeed)
        {
            starBarrage.SetStarSpeed();
            hasSetSpeed = true;
        }
    }
}
