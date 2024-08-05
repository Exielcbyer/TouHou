using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public SoundDetailsList_SO soundDetailsData;
    public GameObject soundPrefab;

    public AudioSource BGMSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private void OnEnable()
    {
        EventHandler.AddEventListener<SoundName>("PlayFXSource", OnPlayFXSourceEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<SoundName>("PlayFXSource", OnPlayFXSourceEvent);
    }

    private void OnPlayFXSourceEvent(SoundName soundName)// ������Ч��������Ч���ȴ���SoundDetails������
    {
        var soundDetails = soundDetailsData.GetSoundDeatils(soundName);
        if (soundDetails != null)
        {
            //�����������Ч���岢��ʼ��
            GameObject sound = ObjectPool.Instance.GetObject(soundPrefab);
            sound.GetComponent<Sound>().SetSound(soundDetails);
        }
    }

    public void PlayBGMSource(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }
}
