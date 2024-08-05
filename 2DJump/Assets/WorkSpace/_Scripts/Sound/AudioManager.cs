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

    private void OnPlayFXSourceEvent(SoundName soundName)// 传入音效名播放音效，比传入SoundDetails更方便
    {
        var soundDetails = soundDetailsData.GetSoundDeatils(soundName);
        if (soundDetails != null)
        {
            //对象池生成音效物体并初始化
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
