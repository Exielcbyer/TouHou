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
    /// ���س���������Ϊ����
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);//����Ŀ�곡��

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newScene);//���ü���
    }

    /// <summary>
    /// �����л�
    /// </summary>
    /// <param name="sceneName">Ŀ�곡��</param>
    /// <param name="targetPosition">Ŀ��λ��</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPos)
    {
        //EventHandler.CallBeforeSceneUnloadEvent();
        EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//���뵭���¼�
        yield return new WaitForSeconds(1f);
        EventHandler.TriggerEvent<Vector3>("MovePlayerPos", targetPos);//�ƶ����������¼�
        if (SceneManager.GetActiveScene().name != "PersistentScene")//����Ϸ���̣�����������Ϸ����
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//ж�ص�ǰ����
        }
        yield return LoadSceneSetActive(sceneName);//���س���������Ϊ����
        ObjectPool.Instance.ClearObjectPool();//��ն����
        EventHandler.TriggerEvent("AfterSceneLoadEvent");
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.clear, 1f);//���뵭���¼�
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
    /// ж�ص�ǰ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator Unload()
    {
        EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//���뵭���¼�
        yield return new WaitForSeconds(1f);
        if (SceneManager.GetActiveScene().name != "PersistentScene")//����Ϸ���̣�����������Ϸ����
        {
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//ж�ص�ǰ����
        }
        EventHandler.TriggerEvent("AfterReturnMainEvent");
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.clear, 1f);//���뵭���¼�
    }
}
