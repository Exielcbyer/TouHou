using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    public string startSceneName;

    private void OnEnable()
    {
        EventHandler.AddEventListener<Vector3>("StartNewGameEvent", OnStartNewGameEvent);
        EventHandler.AddEventListener("ReturnMainEvent", OnReturnMainEvent);
        EventHandler.AddEventListener<string, Vector3>("TransitionScene", OnTransitionEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<Vector3>("StartNewGameEvent", OnStartNewGameEvent);
        EventHandler.RemoveEventListener("ReturnMainEvent", OnReturnMainEvent);
        EventHandler.RemoveEventListener<string, Vector3>("TransitionScene", OnTransitionEvent);
    }

    private void Start()
    {
        SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
    }

    private void OnStartNewGameEvent(Vector3 targetPos)
    {
        StartCoroutine(Transition(startSceneName, targetPos));
    }

    private void OnReturnMainEvent()
    {
        StartCoroutine(Unload());
    }

    private void OnTransitionEvent(string sceneToGo, Vector3 targetPos)
    {
        StartCoroutine(Transition(sceneToGo, targetPos));
    }

    /// <summary>
    /// 加载场景并设置为激活
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);//加载目标场景

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newScene);//设置激活
    }

    /// <summary>
    /// 场景切换
    /// </summary>
    /// <param name="sceneName">目标场景</param>
    /// <param name="targetPosition">目标位置</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPos)
    {
        //EventHandler.CallBeforeSceneUnloadEvent();
        EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//淡入淡出事件
        yield return new WaitForSeconds(1f);
        EventHandler.TriggerEvent<Vector3>("MovePlayerPos", targetPos);//移动人物坐标事件
        if (SceneManager.GetActiveScene().name != "PersistentScene")//在游戏过程，加载另外游戏进度
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//卸载当前场景
        }
        yield return LoadSceneSetActive(sceneName);//加载场景并设置为激活
        ObjectPool.Instance.ClearObjectPool();//清空对象池
        EventHandler.TriggerEvent("AfterSceneLoadEvent");
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.clear, 1f);//淡入淡出事件
        yield return new WaitForSeconds(0.2f);
        if (sceneName == "Scene 1")
        {
            yield return new WaitForSeconds(1f);
            EventHandler.TriggerEvent("FilmShadeEvent", 100f);
            yield return new WaitForSeconds(0.5f);
            EventHandler.TriggerEvent<DialogueName>("DialogueShowEvent", DialogueName.Prologue);
        }
        else
        {
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
        }
    }

    /// <summary>
    /// 卸载当前场景
    /// </summary>
    /// <returns></returns>
    private IEnumerator Unload()
    {
        EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//淡入淡出事件
        yield return new WaitForSeconds(1f);
        if (SceneManager.GetActiveScene().name != "PersistentScene")//在游戏过程，加载另外游戏进度
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//卸载当前场景
        }
        EventHandler.TriggerEvent("AfterReturnMainEvent");
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.clear, 1f);//淡入淡出事件
    }
}
