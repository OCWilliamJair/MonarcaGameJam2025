using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;
public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera playerCamera;             
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private InteractableBase currentFocus;
    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.actions["Interact"].performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        input.actions["Interact"].performed -= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (currentFocus != null && currentFocus.canInteract)
        {
            currentFocus.Interact();
        }
    }

    private void Update()
    {
        if (PlayerActionBlocker.Instance != null && PlayerActionBlocker.Instance.IsBlocked(PlayerAction.Interact))
            return;

        if (playerCamera == null) return;

        // Raycast desde el centro de la pantalla
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green); // Para debug en editor

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            InteractableBase interactable = hit.collider.GetComponent<InteractableBase>();

            if (interactable != null)
            {
                if (currentFocus != interactable)
                {
                    if (currentFocus != null)
                        currentFocus.OnLoseFocus();

                    currentFocus = interactable;
                    currentFocus.OnFocus();
                }
            }
            else
            {
                ClearFocus();
            }
        }
        else
        {
            ClearFocus();
        }
    }

    private void ClearFocus()
    {
        if (currentFocus != null)
        {
            currentFocus.OnLoseFocus();
            currentFocus = null;
        }
    }
}
