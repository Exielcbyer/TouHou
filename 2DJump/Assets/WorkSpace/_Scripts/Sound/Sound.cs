using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void SetSound(SoundDeatils soundDeatils)
    {
        audioSource.clip = soundDeatils.soundClip;
        audioSource.volume = soundDeatils.soundVolume;
        audioSource.pitch = Random.Range(soundDeatils.soundPithMin, soundDeatils.soundPithMax);
        audioSource.Play();

        StartCoroutine(IEnumeratorDestroy(soundDeatils.soundClip.length));
    }

    private IEnumerator IEnumeratorDestroy(float duration)
    {
        yield return new WaitForSeconds(duration);
        ObjectPool.Instance.PushObject(gameObject);
    }
}
