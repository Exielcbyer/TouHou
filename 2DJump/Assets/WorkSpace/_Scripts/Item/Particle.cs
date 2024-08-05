using System.Collections;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        //Invoke("Destroy", 5f);
        StartCoroutine(IEnumeratorDestroy());
    }

    IEnumerator IEnumeratorDestroy()
    {
        yield return new WaitForSeconds(particle.main.duration);
        if (gameObject.activeSelf)
            ObjectPool.Instance.PushObject(gameObject);
    }
}
