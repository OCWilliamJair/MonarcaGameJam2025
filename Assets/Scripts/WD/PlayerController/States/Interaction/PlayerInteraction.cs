using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;
public class PlayerInteraction : MonoBehaviour
{
    public Transform cameraHolder;
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
        if (currentFocus != null)
        {
            currentFocus.Interact();
        }
    }

    private void Update()
    {
        if (PlayerActionBlocker.Instance != null && PlayerActionBlocker.Instance.IsBlocked(PlayerAction.Interact))
            return;

        // Raycast para detectar objeto interactuable
        Ray ray = new Ray(cameraHolder.position, cameraHolder.forward);
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
