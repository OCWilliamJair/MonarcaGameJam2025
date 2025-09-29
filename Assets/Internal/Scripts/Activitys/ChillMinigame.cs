using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChillMinigame : ActivityBase
{
    public enum SpawnMode { HorizontalOnly, AllDirections }

    [SerializeField] private CinemachineCamera _camera;

    [Header("UI Canvases")]
    [SerializeField] private GameObject CanvasDesktop;
    [SerializeField] private GameObject CanvasGame;
    [SerializeField] private GameObject CanvasWin;
    [SerializeField] private GameObject CanvasRetry;
    [SerializeField] private GameObject CanvasTutorial;

    [Header("Jugador")]
    [SerializeField] private RectTransform player;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashDistance = 100f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private Image playerImage;

    [Header("Proyectiles")]
    [SerializeField] private RectTransform projectilePrefab;
    [SerializeField] private RectTransform projectileParent;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private int totalProjectiles = 10;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private SpawnMode spawnMode = SpawnMode.HorizontalOnly;
    [SerializeField] private float maxDiagonalOffset = 100f;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private int maxHits = 3;

    [Header("Input System")]
    [SerializeField] private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction dashAction;
    private InputAction nextAction;

    [Header("Sounds")]
    [SerializeField] private SoundData hitSound;
    [SerializeField] private SoundData CompleteSound;
    [SerializeField] private SoundData FailedSound;

    [SerializeField] private RectTransform canvasRect;

    private int currentHits = 0;
    private int spawnedProjectiles = 0;
    private bool gameRunning = false;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private bool gameComplete = false;
    private Vector2 moveVector;

    protected override void Awake()
    {
        base.Awake();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        dashAction = playerInput.actions["Approve"];
        nextAction = playerInput.actions["Next"];
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.performed += ctx => moveVector = ctx.ReadValue<Vector2>();
            moveAction.canceled += ctx => moveVector = Vector2.zero;
        }

        if (dashAction != null) dashAction.performed += ctx => Dash(moveVector).Forget();
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= ctx => moveVector = ctx.ReadValue<Vector2>();
            moveAction.canceled -= ctx => moveVector = Vector2.zero;
        }

        if (dashAction != null) dashAction.performed -= ctx => Dash(moveVector).Forget();
    }

    private void Update()
    {
        if (!gameRunning || gameComplete) return;

        // Movimiento normal
        Vector3 move = new Vector3(moveVector.x, moveVector.y, 0f);
        player.localPosition += move * moveSpeed * Time.deltaTime;

        // Limitar dentro del canvas
        Vector3 clamped = player.localPosition;
        Vector2 halfSize = canvasRect.rect.size / 2 - player.rect.size / 2;
        clamped.x = Mathf.Clamp(clamped.x, -halfSize.x, halfSize.x);
        clamped.y = Mathf.Clamp(clamped.y, -halfSize.y, halfSize.y);
        player.localPosition = clamped;
    }

    public override void ActivityProcess()
    {
        CanvasDesktop.SetActive(false);
        CanvasGame.SetActive(true);
        CameraManager.Instance.SwitchCamera(_camera, true, 0.5f);
        GameLoop().Forget();
    }

    private async UniTaskVoid GameLoop()
    {
        if (gameComplete) return;

        while (!gameComplete)
        {
            await StartGame();

            if (currentHits < maxHits) // Victoria
            {
                CanvasWin.SetActive(true);
                await UniTask.Delay(3000);
                CanvasWin.SetActive(false);
                CanvasDesktop.SetActive(true);
                CanvasGame.SetActive(false);
                PlayerActionBlocker.Instance.UnblockAll();
                gameRunning = false;
                gameComplete = true;
                CameraManager.Instance.ReturnToLastCamera(0.5f);
                CompleteActivity();
            }
            else // Derrota
            {
                CanvasRetry.SetActive(true);
                nextAction.Reset();
                await UniTask.WaitUntil(() => nextAction.triggered);
                CanvasRetry.SetActive(false);
                ResetGame();
            }
        }
    }

    private async UniTask StartGame()
    {
        if (gameComplete) return;

        PlayerActionBlocker.Instance.BlockAll();
        CanvasTutorial.SetActive(true);
        await UniTask.WaitUntil(() => nextAction.triggered);
        CanvasTutorial.SetActive(false);
        nextAction.Reset();
        ResetGame();
        gameRunning = true;
        CanvasGame.SetActive(true);

        while (spawnedProjectiles < totalProjectiles && gameRunning)
        {
            SpawnProjectile();
            spawnedProjectiles++;
            await UniTask.Delay(System.TimeSpan.FromSeconds(spawnInterval));
        }
    }

    private void SpawnProjectile()
    {
        if (gameComplete) return;

        RectTransform proj = Instantiate(projectilePrefab, projectileParent);

        Vector2 projHalfSize = proj.rect.size / 2f;
        float halfWidth = canvasRect.rect.width / 2f - projHalfSize.x;
        float xPos = Random.Range(-halfWidth, halfWidth);
        float yPos = canvasRect.rect.height / 2f + projHalfSize.y;
        proj.localPosition = new Vector3(xPos, yPos, 0f);

        if (spawnMode == SpawnMode.HorizontalOnly)
            MoveProjectileVertical(proj).Forget();
        else
            MoveProjectileDiagonal(proj).Forget();
    }

    private async UniTaskVoid MoveProjectileVertical(RectTransform proj)
    {
        if (gameComplete) return;

        Vector2 projHalfSize = proj.rect.size / 2f;
        float lowerLimit = -canvasRect.rect.height / 2f - projHalfSize.y;

        while (proj != null && proj.localPosition.y > lowerLimit && gameRunning)
        {
            proj.localPosition += Vector3.down * projectileSpeed * Time.deltaTime;

            if (!isDashing && RectsOverlap(player, proj))
            {
                OnHit(proj);
                return;
            }

            await UniTask.Yield();
        }

        if (proj != null) Destroy(proj.gameObject);
    }

    private async UniTaskVoid MoveProjectileDiagonal(RectTransform proj)
    {
        if (gameComplete) return;

        Vector2 projHalfSize = proj.rect.size / 2f;
        Vector2 canvasHalf = canvasRect.rect.size / 2f;

        // Dirección inicial: diagonal hacia abajo
        Vector2 dir = new Vector2(Random.Range(-maxDiagonalOffset, maxDiagonalOffset), -1f).normalized;

        while (proj != null && proj.localPosition.y > -canvasHalf.y - projHalfSize.y && gameRunning)
        {
            // Movimiento
            Vector3 pos = proj.localPosition + (Vector3)(dir * projectileSpeed * Time.deltaTime);

            // Rebote lateral
            if (pos.x - projHalfSize.x < -canvasHalf.x || pos.x + projHalfSize.x > canvasHalf.x)
            {
                dir.x *= -1;
                pos.x = Mathf.Clamp(pos.x, -canvasHalf.x + projHalfSize.x, canvasHalf.x - projHalfSize.x);
            }

            proj.localPosition = pos;

            if (!isDashing && RectsOverlap(player, proj))
            {
                proj.DOKill();
                OnHit(proj);
                return;
            }

            await UniTask.Yield();
        }

        if (proj != null) Destroy(proj.gameObject);
    }

    private void OnHit(RectTransform proj)
    {
        if (gameComplete) return;

        if (proj != null)
        {
            proj.DOKill();
            Destroy(proj.gameObject);
        }

        currentHits++;
        HitFeedback();

        if (currentHits >= maxHits)
            gameRunning = false;
    }

    private bool RectsOverlap(RectTransform a, RectTransform b)
    {
        Rect rectA = new Rect(a.localPosition - (Vector3)(a.rect.size / 2), a.rect.size);
        Rect rectB = new Rect(b.localPosition - (Vector3)(b.rect.size / 2), b.rect.size);
        return rectA.Overlaps(rectB);
    }

    private void ResetGame()
    {
        if (gameComplete) return;

        foreach (Transform child in projectileParent)
        {
            if (child != player)
                Destroy(child.gameObject);
        }

        currentHits = 0;
        spawnedProjectiles = 0;
        gameRunning = false;
        isDashing = false;
        dashOnCooldown = false;
        statusText.text = "";
    }

    private async UniTaskVoid Dash(Vector3 direction)
    {
        if (gameComplete || !gameRunning) return;

        if (direction == Vector3.zero) direction = Vector3.up;

        isDashing = true;
        dashOnCooldown = true;

        playerImage.DOFade(0.3f, 0.1f).SetLoops(4, LoopType.Yoyo);

        Vector3 startPos = player.localPosition;
        Vector3 targetPos = startPos + direction.normalized * dashDistance;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            player.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / dashDuration);
            await UniTask.Yield();
        }

        isDashing = false;
        await UniTask.Delay(System.TimeSpan.FromSeconds(dashCooldown));
        dashOnCooldown = false;

        playerImage.DOFade(1f, 0.1f);
    }

    private void HitFeedback()
    {
        if (gameComplete) return;
        player.DOShakePosition(0.3f, strength: new Vector3(15f, 15f, 0f), vibrato: 20);
        playerImage.DOColor(Color.red, 0.1f).OnComplete(() =>
        {
            playerImage.DOColor(Color.white, 0.2f);
        });
    }
}
