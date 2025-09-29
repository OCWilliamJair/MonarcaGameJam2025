using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SlidingDoors : InteractableBase
{
    [SerializeField] private SoundData _sound;

    [Header("Configuración Puerta")]
    [SerializeField] private Transform doorTransform; // El objeto de la puerta
    [SerializeField] private Vector3 slideDirection = Vector3.right; // Dirección del deslizamiento (ej: right, left, forward)
    [SerializeField] private float slideDistance = 2f; // Cuánto se moverá
    [SerializeField] private float duration = 1f; // Tiempo de animación
    [SerializeField] private bool autoClose = false; // Si quieres que se cierre sola
    [SerializeField] private float autoCloseDelay = 3f; // Tiempo antes de cerrarse

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private Tween currentTween;

    protected override void Awake()
    {
        base.Awake();
        if (doorTransform == null) doorTransform = transform;

        closedPosition = doorTransform.localPosition;
        openPosition = closedPosition + (slideDirection.normalized * slideDistance);
    }

    public override void Interact()
    {
        if (!canInteract) return;

        if (isOpen)
        {
            CloseDoor().Forget();
        }
        else
        {
            OpenDoor().Forget();
        }
    }

    private async UniTaskVoid OpenDoor()
    {
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

        currentTween = doorTransform.DOLocalMove(openPosition, duration).SetEase(Ease.OutCubic);
        isOpen = true;

        if (autoClose)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(autoCloseDelay));
            CloseDoor().Forget();
        }

        AudioManager.Instance.Play(_sound.name, transform.position);
    }

    public async UniTaskVoid CloseDoor()
    {
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

        currentTween = doorTransform.DOLocalMove(closedPosition, duration).SetEase(Ease.InCubic);
        isOpen = false;
        await UniTask.CompletedTask;

        AudioManager.Instance.Play(_sound.name, transform.position);
    }

    public void close()
    {
        CloseDoor().Forget();
    }
}
