using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [Header("主界面UI")]
    public GameObject mainUI;
    [Header("游戏UI")]
    public GameObject gameCanvas;
    public GameObject fadeCanvas;
    public GameObject dialogueCanvas;
    [Header("菜单UI")]
    public GameObject menuButton;
    public GameObject menuUI;

    [Header("状态UI")]
    public GameObject BossHolder;
    public Slider bossHealthBar;
    public Image bossHealthImage;
    public Image playerHealthImage;
    public Image playerEnergyImage;
    public Image holdAttackImage;
    public List<GameObject> enhancementGroup;
    public GameObject drillingBit;
    public GameObject convoluteShield;

    [Header("淡入淡出UI")]
    public Image fadeImage;//场景切换淡入淡出UI

    [Header("电影黑框")]
    public Transform upFilmShade;
    public Transform downFilmShade;

    private float barSpeed = 0.3f;
    private float healthPerformSpeed = 25f;
    private bool performOver;

    private void OnEnable()
    {
        EventHandler.AddEventListener<float, float>("UpdateBossHealth", OnUpdateBossHealth);
        EventHandler.AddEventListener<float, float>("UpdatePlayerHealth", OnUpdatePlayerHealth);
        EventHandler.AddEventListener<float, float> ("UpdatePlayerEnergy", OnUpdatePlayerEnergy);
        EventHandler.AddEventListener<Color,float>("FadeEvent", OnFadeEvent);
        EventHandler.AddEventListener<float>("FilmShadeEvent", OnFilmShadeEvent);
        EventHandler.AddEventListener<int>("UpdateEnhancementPointsEvent", OnUpdateEnhancementPointsEvent);
        EventHandler.AddEventListener<bool>("UpdateDrillingBitEvent", OnUpdateDrillingBitEvent);
        EventHandler.AddEventListener<bool>("UpdateConvoluteShieldEvent", OnUpdateConvoluteShieldEvent);
        EventHandler.AddEventListener("AfterSceneLoadEvent", OnAfterSceneLoadEvent);
        EventHandler.AddEventListener("AfterReturnMainEvent", OnAfterReturnMainEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<float, float>("UpdateBossHealth", OnUpdateBossHealth);
        EventHandler.RemoveEventListener<float, float>("UpdatePlayerHealth", OnUpdatePlayerHealth);
        EventHandler.RemoveEventListener<float, float>("UpdatePlayerEnergy", OnUpdatePlayerEnergy);
        EventHandler.RemoveEventListener<Color, float>("FadeEvent", OnFadeEvent);
        EventHandler.RemoveEventListener<float>("FilmShadeEvent", OnFilmShadeEvent);
        EventHandler.RemoveEventListener<int>("UpdateEnhancementPointsEvent", OnUpdateEnhancementPointsEvent);
        EventHandler.RemoveEventListener<bool>("UpdateDrillingBitEvent", OnUpdateDrillingBitEvent);
        EventHandler.RemoveEventListener<bool>("UpdateConvoluteShieldEvent", OnUpdateConvoluteShieldEvent);
        EventHandler.RemoveEventListener("AfterSceneLoadEvent", OnAfterSceneLoadEvent);
        EventHandler.RemoveEventListener("AfterReturnMainEvent", OnAfterReturnMainEvent);
    }

    private void Update()
    {
        UpdateAttackHold();
    }

    public void SetBossHealth(float maxHealth)
    {
        bossHealthBar.maxValue = maxHealth;
    }

    private void UpdateAttackHold()
    {
        if (Input.GetKey(KeyCode.X) && !Input.GetKey(KeyCode.Z) && holdAttackImage.fillAmount < 1)
            holdAttackImage.fillAmount += Time.deltaTime * 2;
        else if (Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X) && holdAttackImage.fillAmount < 1)
            holdAttackImage.fillAmount += Time.deltaTime;
        else if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X) && holdAttackImage.fillAmount > 0) 
            holdAttackImage.fillAmount -= Time.deltaTime * 2;

    }

    public IEnumerator BossHealthPerform(float maxHealth)
    {
        yield return new WaitForSeconds(0.5f);
        BossHolder.SetActive(true);
        bossHealthBar.gameObject.SetActive(true);
        while (bossHealthBar.value < maxHealth)
        {
            healthPerformSpeed += Time.deltaTime * 100f;
            bossHealthBar.value += Time.deltaTime * healthPerformSpeed;
            yield return null;
        }
        bossHealthBar.value = maxHealth;
        performOver = true;
    }

    private void SetCanvasActive(bool isActive)
    {
        gameCanvas.SetActive(isActive);
        fadeCanvas.SetActive(isActive);
        dialogueCanvas.SetActive(isActive);
    }

    #region UI事件
    private void OnUpdateBossHealth(float health, float maxHealth)
    {
        if (performOver)
        {
            bossHealthBar.value = health;
            bossHealthImage.DOFillAmount(health / maxHealth, barSpeed);
        }
    }

    private void OnUpdatePlayerHealth(float health, float maxHealth)
    {
        playerHealthImage.DOFillAmount(health / maxHealth, barSpeed);
    }

    private void OnUpdatePlayerEnergy(float energy, float maxEnergy)
    {
        playerEnergyImage.DOFillAmount(energy / maxEnergy, barSpeed);
    }

    private void OnFadeEvent(Color targetColor, float duration)
    {
        fadeImage.DOBlendableColor(targetColor, duration);
    }

    private void OnFilmShadeEvent(float offset)
    {
        upFilmShade.DOMove(upFilmShade.position - new Vector3(0, offset, 0), 1f);
        downFilmShade.DOMove(downFilmShade.position + new Vector3(0, offset, 0), 1f);
    }

    private void OnAfterSceneLoadEvent()
    {
        menuButton.SetActive(true);
        menuUI.SetActive(false);
        mainUI.SetActive(false);
    }

    private void OnAfterReturnMainEvent()
    {
        mainUI.SetActive(true);
    }

    public void OnUpdateEnhancementPointsEvent(int enhancementPoints)
    {
        for (int i = 0; i < enhancementGroup.Count; i++)
        {
            enhancementGroup[i].SetActive(false);
        }
        for (int i = 0; i < enhancementPoints; i++)
        {
            enhancementGroup[i].SetActive(true);
        }
    }

    public void OnUpdateDrillingBitEvent(bool isActive)
    {
        drillingBit.SetActive(isActive);
    }

    public void OnUpdateConvoluteShieldEvent(bool isActive)
    {
        convoluteShield.SetActive(isActive);
    }
    #endregion
    #region 按钮事件
    public void OnStartGameButtonClicked()
    {
        EventHandler.TriggerEvent<Vector3>("StartNewGameEvent", GameManager.Instance.startPos);
        SetCanvasActive(true);
    }

    public void OnExitGameButtonClicked()
    {
        Application.Quit();
    }

    public void OnMenuButtonClicked()
    {
        menuUI.SetActive(!menuUI.activeSelf);
        if (GameManager.Instance.gameState == GameState.Pause)
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
        else
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
    }

    public void OnReturnMainButtonClicked()
    {
        EventHandler.TriggerEvent("ReturnMainEvent");
        StartCoroutine(ReturnMainIEnumerator());
    }

    private IEnumerator ReturnMainIEnumerator()
    {
        dialogueCanvas.SetActive(false);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.black, 1f);//淡入淡出事件
        yield return new WaitForSeconds(1f);
        SetCanvasActive(false);
        EventHandler.TriggerEvent("FilmShadeEvent", -100f);
        yield return new WaitForSeconds(0.5f);
        EventHandler.TriggerEvent<Color, float>("FadeEvent", Color.white, 1f);//淡入淡出事件
    }

    public void OnLoadButtonClicked()
    {
        EventHandler.TriggerEvent("LoadEvent");
        SetCanvasActive(true);
    }
    #endregion
}
