using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private float skipCooldown = 0.3f;
    [SerializeField] private float autoAdvanceTime = 4f;

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isPlaying = false;
    private bool canSkip = false;

    [SerializeField] private PlayerInput playerInput;

    private CancellationTokenSource dialogueCTS;
    private CancellationTokenSource autoAdvanceCTS;

    // ? Cola de diálogos pendientes
    private Queue<DialogueRequest> dialogueQueue = new Queue<DialogueRequest>();

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

    /// <summary>
    /// Llamar desde otros scripts para iniciar un diálogo
    /// </summary>
    public void StartDialogue(DialogueData dialogue, bool blockActionsValue)
    {
        if (isPlaying)
        {
            // Si ya hay un diálogo, lo encolamos
            dialogueQueue.Enqueue(new DialogueRequest(dialogue, blockActionsValue));
        }
        else
        {
            PlayDialogue(dialogue, blockActionsValue).Forget();
        }
    }

    private async UniTaskVoid PlayDialogue(DialogueData dialogue, bool blockActionsValue)
    {
        if (blockActionsValue)
            PlayerActionBlocker.Instance.BlockAction(PlayerActionBlocker.PlayerAction.Move);

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

            // Cancelar cualquier auto-advance anterior
            autoAdvanceCTS?.Cancel();
            autoAdvanceCTS = new CancellationTokenSource();

            // Mostrar línea con efecto máquina de escribir
            await dialogueUI.ShowLine(line.speakerName, line.text, dialogueCTS.Token);

            // Activamos el skip después del cooldown
            await UniTask.Delay((int)(skipCooldown * 1000), cancellationToken: dialogueCTS.Token)
                         .SuppressCancellationThrow();
            canSkip = true;

            // Iniciar auto avance
            AutoAdvance(autoAdvanceCTS.Token).Forget();
        }
        else
        {
            EndDialogue().Forget();
        }
    }

    private async UniTaskVoid AutoAdvance(CancellationToken token)
    {
        await UniTask.Delay((int)(autoAdvanceTime * 1000), cancellationToken: token)
                     .SuppressCancellationThrow();

        if (isPlaying && canSkip && !dialogueUI.IsTyping)
        {
            ShowNextLine();
        }
    }

    private async UniTaskVoid EndDialogue()
    {
        PlayerActionBlocker.Instance.UnblockAction(PlayerActionBlocker.PlayerAction.Move);
        isPlaying = false;

        // Cancelar ambas fuentes
        dialogueCTS?.Cancel();
        autoAdvanceCTS?.Cancel();

        await dialogueUI.Hide();

        currentDialogue = null;

        // ? Revisar si hay más diálogos en la cola
        if (dialogueQueue.Count > 0)
        {
            var next = dialogueQueue.Dequeue();
            PlayDialogue(next.data, next.blockActions).Forget();
        }
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

    // ? Estructura para guardar la info de cada petición
    private struct DialogueRequest
    {
        public DialogueData data;
        public bool blockActions;

        public DialogueRequest(DialogueData d, bool block)
        {
            data = d;
            blockActions = block;
        }
    }
}
