using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class TimeLineDirector : MonoBehaviour
{
    private PlayableDirector director;
    public CinemachineVirtualCamera virtualCamera;
    public List<PlayableAsset> playableAssetList = new List<PlayableAsset>();

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<int>("TimeLinePlayEvent", OnTimeLinePlayEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<int>("TimeLinePlayEvent", OnTimeLinePlayEvent);
    }

    private void OnTimeLinePlayEvent(int index)
    {
        director.playableAsset = playableAssetList[index];
        director.Play();
    }

    public void OnPrologueDialogueSignal()
    {
        EventHandler.TriggerEvent<DialogueName>("DialogueShowEvent", DialogueName.Prologue);
    }

    public void OnWithMarisaDialogueSignal()
    {
        EventHandler.TriggerEvent("FilmShadeEvent", -100f);
        EventHandler.TriggerEvent("BehaviorTreeEnableEvent");
        EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
        StartCoroutine(UIManager.Instance.BossHealthPerform(400));
    }

    public void OnSetCameraFollowSignal()
    {
        virtualCamera.Follow = GameObject.Find("Marisa").transform;
        Invoke(nameof(SetCameraFollowNull), 2f);
    }

    public void OnMarisaAdmissionTriggerSignal()
    {
        EventHandler.TriggerEvent<string>("MarisaTriggerEvent", "Admission");
    }

    public void OnMarisaDefeatTriggerSignal()
    {
        EventHandler.TriggerEvent<string>("MarisaTriggerEvent", "Defeat");
    }

    private void SetCameraFollowNull()
    {
        virtualCamera.Follow = null;
    }

    public void OnConquerSignal()
    {
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//淡入淡出事件
        EventHandler.TriggerEvent("FilmShadeEvent", -100f);
        Invoke(nameof(ConquerOver), 1f);
    }

    private void ConquerOver()
    {
        ObjectPool.Instance.ClearObjectPool();
        EventHandler.TriggerEvent("ConquerOver");
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.clear, 1f);//淡入淡出事件
    }
}