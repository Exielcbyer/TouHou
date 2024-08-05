using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDeatils> soundDeatilList;

    /// <summary>
    /// 根据名字返回声音数据
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public SoundDeatils GetSoundDeatils(SoundName name)
    {
        return soundDeatilList.Find(s => s.soundName == name);
    }
}

[System.Serializable]
public class SoundDeatils
{
    public SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)]
    public float soundPithMin;//最低音调
    [Range(0.1f, 1.5f)]
    public float soundPithMax;//最高音调
    [Range(0.1f, 1.5f)]
    public float soundVolume;//声音大小
}
