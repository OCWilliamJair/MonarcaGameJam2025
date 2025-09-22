using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    [SerializeField] private float acceleration = 10f; // Aceleración al empezar a caminar.
    [SerializeField] private float deceleration = 15f; // Desaceleración al dejar de caminar.
    [SerializeField] private float maxSpeed = 5f; // Velocidad máxima para limitar el movimiento.

    private CharacterController controller;
    private PlayerInput input;
    private Vector2 currentMoveInput; // El valor de input del jugador.
    private Vector2 smoothMoveInput; // Un valor suavizado para la aceleración y desaceleración.

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
        currentMoveInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (PlayerActionBlocker.Instance != null && PlayerActionBlocker.Instance.IsBlocked(PlayerAction.Move))
            return;

        if (currentMoveInput.magnitude > 0)
        {
            // Aceleración
            smoothMoveInput = Vector2.MoveTowards(smoothMoveInput, currentMoveInput, acceleration * Time.deltaTime);
        }
        else
        {
            // Desaceleración
            smoothMoveInput = Vector2.MoveTowards(smoothMoveInput, currentMoveInput, deceleration * Time.deltaTime);
        }

        // Limita la velocidad máxima.
        if (smoothMoveInput.magnitude > 1)
        {
            smoothMoveInput.Normalize();
        }

        // Calcula la dirección del movimiento en el mundo 3D.
        Vector3 direction = transform.right * smoothMoveInput.x + transform.forward * smoothMoveInput.y;

        // Mueve el CharacterController aplicando la velocidad máxima.
        controller.Move(direction * maxSpeed * Time.deltaTime);
    }
}
