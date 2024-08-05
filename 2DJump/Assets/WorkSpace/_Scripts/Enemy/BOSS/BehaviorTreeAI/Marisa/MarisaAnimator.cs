using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarisaAnimator : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<string>("MarisaTriggerEvent", MarisaAnimatorTrigger);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<string>("MarisaTriggerEvent", MarisaAnimatorTrigger);
    }

    private void MarisaAnimatorTrigger(string triggerName)
    {
        switch (triggerName)
        {
            case "Admission":
                anim.SetTrigger("Admission");
                break;
            case "Defeat":
                anim.SetTrigger("Defeat");
                break;
            default:
                break;
        }
    }
}
