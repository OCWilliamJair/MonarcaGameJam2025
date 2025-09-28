using UnityEngine;

public class DialogueContainer : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueStart;

    [SerializeField] private DialogueData dialogueComplete;

    [SerializeField] private bool blockActionStart;

    [SerializeField] private bool blockActionComplete;
    public void StarDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueStart, blockActionStart);
    }

    public void CompleteDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogueComplete, blockActionComplete);
    }
}
