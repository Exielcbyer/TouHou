using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlashParticles : MonoBehaviour
{
    public GameObject slashObject;
    private ParticleSystem particle;

    Transform player => GameObject.Find("Player").transform;

    private void Awake()
    {
        particle = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        //Invoke("Destroy", 5f);
        StartCoroutine(IEnumeratorDestroy());
    }

    public void Init(float intputX, float inputY, float localScaleX)
    {
        Vector2 direction = new Vector2(intputX, inputY).normalized;
        if(intputX == 0 && inputY == 0)
            direction = new Vector2(localScaleX, inputY).normalized;
        transform.right = direction;
    }

    private void ReSet()
    {
        transform.position = player.position;
    }

    IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(particle.main.duration / 2);
        slashObject.SetActive(true);
        yield return new WaitForSeconds(slashObject.GetComponent<ParticleSystem>().main.duration);
        slashObject.SetActive(false);
        if (gameObject.activeSelf)
        {
            ReSet();
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
