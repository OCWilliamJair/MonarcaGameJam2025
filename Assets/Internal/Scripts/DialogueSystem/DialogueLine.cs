using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;   // Nombre del que habla
    [TextArea(2, 5)]
    public string text;          // Texto del diálogo
}
