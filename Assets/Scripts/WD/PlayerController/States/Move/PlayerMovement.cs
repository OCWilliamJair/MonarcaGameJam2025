using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    [SerializeField] private float speed = 3f;

    private CharacterController controller;
    private PlayerInput input;
    private Vector2 moveInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.actions["Move"].performed += OnMove;
        input.actions["Move"].canceled += OnMove;
    }

    private void OnDisable()
    {
        input.actions["Move"].performed -= OnMove;
        input.actions["Move"].canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (PlayerActionBlocker.Instance != null && PlayerActionBlocker.Instance.IsBlocked(PlayerAction.Move))
            return;

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(direction * speed * Time.deltaTime);
    }
}
