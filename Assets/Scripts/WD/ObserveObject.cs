using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObserveObject : InteractableBase
{
    [SerializeField] private PlayerInput player;

    [SerializeField] CinemachineCamera _camera;

    private bool isActive = false;

    public override void Interact()
    {
        if (isActive) return;
        isActive = true;
        Debug.Log("Interactuando");
        CameraManager.Instance.SwitchCamera(_camera, true, 0.5f);
        PlayerActionBlocker.Instance.BlockAll();     
    }

    private void OnEnable()
    {
        if (player != null)
            player.actions["Cancel"].started += OnCancelPerformed;
    }


    private void OnDisable()
    {
        if (player != null)
            player.actions["Cancel"].started -= OnCancelPerformed;
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;
        Cancel();
    }

    private void Cancel()
    {
        PlayerActionBlocker.Instance.UnblockAll();
        CameraManager.Instance.ReturnToLastCamera(0.5f);
        isActive = false;
    }
}
