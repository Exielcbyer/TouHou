using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
