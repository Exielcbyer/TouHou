using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    // ��ɹ�һ�ζԻ���ı�Ի�����
    public DialogueDetails_SO dialogueEmpty;
    public DialogueDetails_SO dialogueFinish;

    public GameObject dialogueSignObject;

    public DialogueName dialogueName;

    private Stack<DialoguePiece> dialogueEmptyStack;
    private Stack<DialoguePiece> dialogueFinishStack;

    private bool canTalk;// �Ƿ��ڶԻ���Χ��
    private bool isFinish;// �Ƿ��Ѿ��Ի���
    private bool isTalking;// �Ƿ����ڶԻ�

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
            // ������ͣ��Ϸ���¼�
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
            // ������ʾ�Ի����¼�
            EventHandler.TriggerEvent("ShowDialogue", result);
            yield return new WaitForSeconds(1f);
            isTalking = false;
        }
        else
        {
            // ������ʼ��Ϸ���¼�
            EventHandler.TriggerEvent("GameStateChangeEvent", GameState.GamePlay);
            // �����رնԻ����¼�
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
                    // ������ͣ��Ϸ���¼�
                    EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
                    // ���ſ����ݳ�
                    EventHandler.TriggerEvent<int>("TimeLinePlayEvent", 0);
                    break;
                case DialogueName.WithMarisa:
                    // ������ͣ��Ϸ���¼�
                    EventHandler.TriggerEvent("GameStateChangeEvent", GameState.Pause);
                    EventHandler.TriggerEvent("FilmShadeEvent", 100f);
                    // ���Ž���ս���ݳ�
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
