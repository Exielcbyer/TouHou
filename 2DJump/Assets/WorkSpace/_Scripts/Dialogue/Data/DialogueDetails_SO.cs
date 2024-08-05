using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDetails_SO", menuName = "Dialogue/DialogueDetails")]
public class DialogueDetails_SO : ScriptableObject
{
    public List<DialoguePiece> dialogueList;
}

[Serializable]
public class DialoguePiece
{
    [Header("∂‘ª∞œÍ«È")]
    public Sprite faceImage;
    public bool OnLeft;
    public string name;
    [TextArea]
    public string dialogueText;
}
