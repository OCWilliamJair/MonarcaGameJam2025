using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[System.Serializable]
public class EmailData
{
    public Sprite emailImage;
    public string correctLabel; 
}

public class WorkMinigame : ActivityBase
{
    [SerializeField] private SoundData _hitSound;

    [SerializeField] private SoundData _correctSound;

    [Header("UI Elements")]
    [SerializeField] private GameObject MainCanvas;
    [SerializeField] private GameObject retryCanvas;
    [SerializeField] private GameObject WorkingComplete;
    [SerializeField] private GameObject DesktopCanvas;
    public Image emailDisplay;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public GameObject tutorialContainer;
    [SerializeField] private CinemachineCamera _camera;

    [Header("Game Settings")]
    public EmailData[] emails;
    public int maxErrors = 3;
    public float timePerEmail = 3f;

    [SerializeField] private PlayerInput playerInput;
    private InputAction approveAction;
    private InputAction rejectAction;
    private InputAction skipInterfaces;

    private int currentEmailIndex = 0;
    private int errors = 0;
    private bool emailProcessed = false;
    private bool gameIsStarted = false;

    [Header("Animation Settings")]
    public float feedbackScaleDuration = 0.15f;
    public float feedbackFadeDuration = 0.3f;
    public float scorePopDuration = 0.2f;
    public float emailPopDuration = 0.2f;
    public float emailShakeDuration = 0.25f;

    protected override void Awake()
    {
        base.Awake();
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("WorkMinigame: playerInput no asignado ni encontrado en GetComponent<PlayerInput>()");
            return;
        }

        approveAction = playerInput.actions["Approve"];
        rejectAction = playerInput.actions["Cancel"];
        skipInterfaces = playerInput.actions["Next"];
    }

    private void OnEnable()
    {
        if (approveAction != null) approveAction.performed += OnApprove;
        if (rejectAction != null) rejectAction.performed += OnReject;
    }

    private void OnDisable()
    {
        if (approveAction != null) approveAction.performed -= OnApprove;
        if (rejectAction != null) rejectAction.performed -= OnReject;
    }

    private void OnApprove(InputAction.CallbackContext ctx) => Choose("Importante");
    private void OnReject(InputAction.CallbackContext ctx) => Choose("Spam");

    protected override void RestartValues()
    {
        base.RestartValues();
        MainCanvas.SetActive(false);
    }

    public override void ActivityProcess()
    {
        RunGame().Forget();
    }

    public void SetNewDificulty()
    {
        timePerEmail -= (timePerEmail * 0.10f);
        maxErrors--;
    }

    private async UniTask RunGame()
    {
        DesktopCanvas.SetActive(false);
        CameraManager.Instance.SwitchCamera(_camera, true, 0.5f);
        PlayerActionBlocker.Instance.BlockAll();
        await UniTask.Delay(2000);
        MainCanvas.SetActive(true);
        tutorialContainer.SetActive(true);
        gameIsStarted = true;
        await UniTask.WaitUntil(() => skipInterfaces.triggered);
        tutorialContainer.SetActive(false);
        await UniTask.Delay(1000);

        bool gameCompleted = false;

        while (!gameCompleted)
        {
            // Reinicia valores antes de empezar la partida
            RestartGameValues();

            feedbackText.text = "";
            scoreText.text = $"Errores: {errors}/{maxErrors}";

            while (currentEmailIndex < emails.Length && errors < maxErrors)
            {
                ShowEmail(currentEmailIndex);
                emailProcessed = false;

                float timer = 0f;

                while (!emailProcessed && timer < timePerEmail)
                {
                    timer += Time.deltaTime;
                    await UniTask.Yield();
                }

                if (!emailProcessed)
                {
                    feedbackText.DOKill();
                    emailDisplay.transform.DOKill();

                    feedbackText.text = "? Tiempo agotado";
                    ShowFeedbackAnimation(feedbackText, Color.red);

                    // En World Space queda mejor con punch en escala
                    emailDisplay.transform.DOPunchScale(Vector3.one * 0.15f, emailShakeDuration, 6, 0.7f);

                    errors++;
                    scoreText.text = $"Errores: {errors}/{maxErrors}";
                }

                await UniTask.Delay(300);
                currentEmailIndex++;
            }

            if (errors >= maxErrors)
            {
                // Mostrar canvas de retry
                retryCanvas.SetActive(true);

                feedbackText.DOKill();
                feedbackText.text = "? Has perdido";
                ShowFeedbackAnimation(feedbackText, Color.red);

                await UniTask.Delay(1500); // dar un respiro antes de esperar input

                // Espera a que presione Next para reiniciar
                await UniTask.WaitUntil(() => skipInterfaces.triggered);

                retryCanvas.SetActive(false);
            }
            else
            {
                feedbackText.DOKill();
                WorkingComplete.SetActive(true);
                await UniTask.Delay(3000);
                WorkingComplete.SetActive(false);
                gameCompleted = true;
                RestartGameValues();
                MainCanvas.SetActive(false);
                CameraManager.Instance.ReturnToLastCamera(0.5f);
                await UniTask.Delay(500);
                PlayerActionBlocker.Instance.UnblockAll();
                gameIsStarted = false;
                DesktopCanvas.SetActive(true);
                CompleteActivity();
            }
        }
    }

    // Reinicia los valores internos del juego para una nueva partida
    private void RestartGameValues()
    {
        currentEmailIndex = 0;
        errors = 0;
        emailProcessed = false;
        feedbackText.text = "";
        scoreText.text = $"Errores: {errors}/{maxErrors}";
    }

    private void ShowEmail(int index)
    {
        emailDisplay.sprite = emails[index].emailImage;
        labelText.text = emails[index].correctLabel;

        // Reinicia animaciones anteriores antes de mostrar correo
        emailDisplay.transform.DOKill();
        emailDisplay.transform.localScale = Vector3.zero;
        emailDisplay.transform.DOScale(1f, emailPopDuration).SetEase(Ease.OutBack);
    }

    private void Choose(string choice)
    {
        if (emailProcessed || !gameIsStarted) return;

        emailProcessed = true;
        string correct = emails[currentEmailIndex].correctLabel;

        // Mata animaciones anteriores para evitar solapamiento
        feedbackText.DOKill();
        emailDisplay.transform.DOKill();

        if (choice == correct)
        {
            feedbackText.text = "? Correcto";
            ShowFeedbackAnimation(feedbackText, Color.green);
        }
        else
        {
            feedbackText.text = "? Incorrecto";
            errors++;
            scoreText.text = $"Errores: {errors}/{maxErrors}";
            ShowFeedbackAnimation(feedbackText, Color.red);

            // Punch en World Space para el correo
            emailDisplay.transform.DOPunchScale(Vector3.one * 0.15f, emailShakeDuration, 6, 0.7f);
            ShowScorePopAnimation();
        }
    }

    private void ShowFeedbackAnimation(TextMeshProUGUI text, Color color)
    {
        text.color = color;
        text.alpha = 0f;
        text.transform.localScale = Vector3.one * 0.001f; // escala inicial pequeña en World Space

        Sequence seq = DOTween.Sequence();
        seq.Append(text.DOFade(1f, 0.1f));
        seq.Join(text.transform.DOScale(Vector3.one * 0.002f, feedbackScaleDuration).SetEase(Ease.OutBack));
        seq.AppendInterval(0.5f);
        seq.Append(text.DOFade(0f, feedbackFadeDuration));
    }

    private void ShowScorePopAnimation()
    {
        scoreText.transform.DOKill();
        scoreText.transform.localScale = Vector3.one;
        scoreText.transform.DOPunchScale(Vector3.one * 0.2f, scorePopDuration, 1, 0.5f);
    }
}
