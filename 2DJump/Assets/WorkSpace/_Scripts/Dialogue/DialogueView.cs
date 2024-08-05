using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// MVC写法，分成model、view、Controller三层，此脚本为View层
public class DialogueView : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image LeftImage;
    [SerializeField] private Text LeftNameText;
    [SerializeField] private Image RightImage;
    [SerializeField] private Text RightNameText;

    private void OnEnable()
    {
        EventHandler.AddEventListener<DialoguePiece>("ShowDialogue", ShowDialogue);
    }


    private void OnDisable()
    {
        EventHandler.RemoveEventListener<DialoguePiece>("ShowDialogue", ShowDialogue);
        panel.SetActive(false);
    }

    private void ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            panel.SetActive(true);
            if (piece.OnLeft)
            {
                LeftImage.sprite = piece.faceImage;
                LeftNameText.text = piece.name;
                LeftImage.gameObject.SetActive(true);
                RightImage.gameObject.SetActive(false);
            }
            else
            {
                RightImage.sprite = piece.faceImage;
                RightNameText.text = piece.name;
                LeftImage.gameObject.SetActive(false);
                RightImage.gameObject.SetActive(true);
            }
            dialogueText.text = string.Empty;
            dialogueText.DOText(piece.dialogueText, 1f);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
