using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    // 完成过一次对话后改变对话内容
    public DialogueDetails_SO dialogueEmpty;
    public DialogueDetails_SO dialogueFinish;

    public GameObject dialogueSignObject;

    public DialogueName dialogueName;

    private Stack<DialoguePiece> dialogueEmptyStack;
    private Stack<DialoguePiece> dialogueFinishStack;

    private bool canTalk;// 是否在对话范围内
    private bool isFinish;// 是否已经对话过
    private bool isTalking;// 是否正在对话

    private void Awake()
    {
        FillDialogueStack();
    }

    private void OnEnable()
    {
        EventHandler.AddEventListener<DialogueName>("DialogueShowEvent", OnDialogueShowEvent);
    }

    private void OnDisable()
    {
        EventHandler.RemoveEventListener<DialogueName>("DialogueShowEvent", OnDialogueShowEvent);
    }

    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.E))
        {
            if (!isFinish)
                ShowDialogue();
            else
            {
                if (GameManager.Instance.Conquer)
                    ShowDialogueFinish();
                else if(!GameManager.Instance.Conquer && dialogueName != DialogueName.WithMarisa)
                    ShowDialogueFinish();
            }
        }
        dialogueSignObject.SetActive(canTalk && !isFinish);
    }

    private void FillDialogueStack()
    {
        dialogueEmptyStack = new Stack<DialoguePiece>();
        dialogueFinishStack = new Stack<DialoguePiece>();
        DialogueModel.Instance.FillDialogueStack(dialogueEmpty, dialogueEmptyStack);
        DialogueModel.Instance.FillDialogueStack(dialogueFinish, dialogueFinishStack);
    }

    public void ShowDialogue()
    {
        if (!isTalking)
            StartCoroutine(DialogueRoutine(dialogueEmptyStack));
    }

    public void ShowDialogueFinish()
    {
        if (!isTalking)
            StartCoroutine(DialogueRoutine(dialogueFinishStack));
    }

    private IEnumerator DialogueRoutine(Stack<DialoguePiece> data)
    {
        isTalking = true;
        if (data.TryPop(out DialoguePiece result))
        {
            // 触发暂停游戏的事件
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
            // 触发显示对话的事件
            EventHandler.TriggerEvent("ShowDialogue", result);
            yield return new WaitForSeconds(1f);
            isTalking = false;
        }
        else
        {
            // 触发开始游戏的事件
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
            // 触发关闭对话的事件
            EventHandler.TriggerEvent("ShowDialogue", (DialoguePiece)null);
            DialogueOver();
            isTalking = false;
            canTalk = false;
        }
    }

    private void DialogueOver()
    {
        if (!isFinish)
        {
            switch (dialogueName)
            {
                case DialogueName.Prologue:
                    // 触发暂停游戏的事件
                    EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
                    // 播放开场演出
                    EventHandler.TriggerEvent<int>("TimeLinePlayEvent", 0);
                    break;
                case DialogueName.WithMarisa:
                    // 触发暂停游戏的事件
                    EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
                    EventHandler.TriggerEvent("FilmShadeEvent", 100f);
                    // 播放进入战斗演出
                    EventHandler.TriggerEvent<int>("TimeLinePlayEvent", 1);
                    break;
                default:
                    break;
            }
            isFinish = true;
        }
        else
        {
            switch (dialogueName)
            {
                case DialogueName.Prologue:
                    EventHandler.TriggerEvent("FilmShadeEvent", -100f);
                    EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
                    break;
                case DialogueName.WithMarisa:
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canTalk = false;
        }
    }

    private void OnDialogueShowEvent(DialogueName dialogueName)
    {
        if (this.dialogueName == dialogueName)
        {
            canTalk = true;
            if (!isFinish)
                ShowDialogue();
            else
            {
                if (GameManager.Instance.Conquer)
                    ShowDialogueFinish();
                else if (!GameManager.Instance.Conquer && dialogueName != DialogueName.WithMarisa)
                    ShowDialogueFinish();
            }
        }
    }
}
