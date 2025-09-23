using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;   // Para controlar fade
    [SerializeField] private RectTransform panel;       // Para mover de abajo hacia arriba
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float showDuration = 0.6f; // Duración animación de aparición
    [SerializeField] private float hideDuration = 0.4f;

    private string currentFullText;
    private CancellationTokenSource typingCTS;

    public bool IsTyping { get; private set; }

    private Vector2 originalPosition;

    private void Awake()
    {
        originalPosition = panel.anchoredPosition;
        canvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Animación de aparición SOLO al inicio del diálogo
    /// </summary>
    public async UniTask ShowPanel(CancellationToken token)
    {
        panel.gameObject.SetActive(true);

        // Reset posición inicial (más abajo de lo normal)
        panel.anchoredPosition = originalPosition + new Vector2(0, -120f);
        canvasGroup.alpha = 0;

        // Animaciones: mover hacia arriba + fade in
        Sequence seq = DOTween.Sequence();
        seq.Append(panel.DOAnchorPos(originalPosition, showDuration).SetEase(Ease.OutCubic));
        seq.Join(canvasGroup.DOFade(1f, showDuration));

        await seq.AsyncWaitForCompletion();
    }

    /// <summary>
    /// Mostrar texto en pantalla (sin animar el panel cada vez).
    /// </summary>
    public async UniTask ShowLine(string speaker, string text, CancellationToken token)
    {
        // Limpiar antes de escribir
        dialogueText.text = "";
        speakerText.text = speaker;
        currentFullText = text;

        typingCTS?.Cancel();
        typingCTS = new CancellationTokenSource();

        await TypeTextAsync(text, typingCTS.Token);
    }

    public async UniTask Hide()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(panel.DOAnchorPos(originalPosition + new Vector2(0, -120f), hideDuration).SetEase(Ease.InCubic));
        seq.Join(canvasGroup.DOFade(0f, hideDuration));

        await seq.AsyncWaitForCompletion();
        dialogueText.text = "";
        speakerText.text = "";
        currentFullText = "";
        panel.gameObject.SetActive(false);
    }

    private async UniTask TypeTextAsync(string text, CancellationToken token)
    {
        dialogueText.text = "";        
        IsTyping = true;

        foreach (char letter in text.ToCharArray())
        {
            if (token.IsCancellationRequested) return;

            dialogueText.text += letter;
            await UniTask.Delay((int)(typingSpeed * 1000), cancellationToken: token);
        }

        IsTyping = false;
    }

    public void SkipTyping()
    {
        typingCTS?.Cancel();
        dialogueText.text = currentFullText;
        IsTyping = false;
    }
}
