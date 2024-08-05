using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public GameObject saveSignObject;

    private bool canSave;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (canSave && Input.GetKeyDown(KeyCode.E))
        {
            EventHandler.TriggerEvent<Vector3>("SaveEvent", transform.position);
        }
        saveSignObject.SetActive(canSave);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canSave = false;
        }
    }
}
