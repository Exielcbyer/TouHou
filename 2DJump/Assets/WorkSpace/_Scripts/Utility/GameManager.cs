using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector3 startPos = new Vector3(-10.34f, -8.33f, 0);
    public GameState gameState;
    public List<GameObject> enemyList;

    public bool Conquer;

    private void OnEnable()
    {
        EventHandler.AddEventListener<GameState>("GameStateChangeEvent", OnGameStateChangeEvent);
        EventHandler.AddEventListener("AfterSceneLoadEvent", OnAfterSceneLoadEvent);
        EventHandler.AddEventListener("BossDeadEvent", OnBossDeadEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<GameState>("GameStateChangeEvent", OnGameStateChangeEvent);
        EventHandler.RemoveEventListener("AfterSceneLoadEvent", OnAfterSceneLoadEvent);
        EventHandler.RemoveEventListener("BossDeadEvent", OnBossDeadEvent);
    }

    private void Start()
    {
        gameState = GameState.Pause;
    }

    private void OnGameStateChangeEvent(GameState gameState)
    {
        this.gameState = gameState;
    }

    private void OnAfterSceneLoadEvent()
    {
        enemyList.Clear();
    }

    private void OnBossDeadEvent()
    {
        Conquer = true;
    }
}
