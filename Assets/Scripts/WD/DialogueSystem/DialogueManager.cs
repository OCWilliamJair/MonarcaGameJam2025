using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private float skipCooldown = 0.3f;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isPlaying = false;
    private bool canSkip = false;

    [SerializeField] private PlayerInput playerInput;

    private CancellationTokenSource dialogueCTS;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.actions["Next"].performed += OnNext;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.actions["Next"].performed -= OnNext;
    }

    public async void StartDialogue(DialogueData dialogue, bool blockAcionsValue)
    {
        if (isPlaying) return;

        if(blockAcionsValue) PlayerActionBlocker.Instance.BlockAll();

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isPlaying = true;
        dialogueCTS = new CancellationTokenSource();

        // Mostrar panel con animación
        await dialogueUI.ShowPanel(dialogueCTS.Token);

        ShowNextLine();
    }

    public async void ShowNextLine()
    {
        if (currentDialogue == null) return;

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            DialogueLine line = currentDialogue.lines[currentLineIndex];
            currentLineIndex++;

            canSkip = false;

            // Mostrar línea con efecto máquina de escribir
            await dialogueUI.ShowLine(line.speakerName, line.text, dialogueCTS.Token);

            // Activamos el skip después del cooldown
            await UniTask.Delay((int)(skipCooldown * 1000), cancellationToken: dialogueCTS.Token);
            canSkip = true;
        }
        else
        {
            EndDialogue();
        }
    }

    private async void EndDialogue()
    {
        PlayerActionBlocker.Instance.UnblockAll();
        isPlaying = false;
        dialogueCTS?.Cancel();

        await dialogueUI.Hide();

        currentDialogue = null;
    }

    private void OnNext(InputAction.CallbackContext ctx)
    {
        if (!isPlaying) return;
        if (!canSkip) return;

        if (dialogueUI.IsTyping)
        {
            dialogueUI.SkipTyping();
        }
        else
        {
            ShowNextLine();
        }
    }

    public bool IsPlaying() => isPlaying;
}
