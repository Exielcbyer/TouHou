using System;
using System.Collections;
using UnityEngine;

public class InteractTest : MonoBehaviour
{
    Collider2D[] colliders = new Collider2D[100];

    private void OnEnable()
    {
        StartCoroutine(IEnumeratorDestroy());
    }

    private void Update()
    {
        Array.Clear(colliders, 0, colliders.Length);
        Physics2D.OverlapCircleNonAlloc(transform.position, 4, colliders);

        foreach (var coll in colliders)
        {
            if (coll == null)
                break;
            if (coll.gameObject.TryGetComponent(out BaseBarrage barrage))
            {
                if(barrage.isFrozen)
                    barrage.Interact();// ²âÊÔµ¯Ä»»¥¶¯
            }
        }
    }

    IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
