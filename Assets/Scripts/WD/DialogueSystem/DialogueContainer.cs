using UnityEngine;

public class DialogueContainer : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;

    [SerializeField] private bool blockAction;
    public void StarDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, blockAction);
    }
}
