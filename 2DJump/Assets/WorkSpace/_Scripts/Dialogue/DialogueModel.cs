using System.Collections.Generic;

public class DialogueModel : Singleton<DialogueModel>
{
    public DialogueDetails_SO dialogueDetails;

    public void FillDialogueStack(DialogueDetails_SO dialogue, Stack<DialoguePiece> dialogueStack)
    {
        for (int i = dialogue.dialogueList.Count - 1; i > -1; i--)
        {
            dialogueStack.Push(dialogue.dialogueList[i]);
        }
    }
}
