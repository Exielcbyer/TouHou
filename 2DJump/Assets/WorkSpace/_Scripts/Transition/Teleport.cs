using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public string sceneToGo;
    public Vector3 targetPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventHandler.TriggerEvent<string, Vector3>("TransitionScene", sceneToGo, targetPos);
        }
    }
}
