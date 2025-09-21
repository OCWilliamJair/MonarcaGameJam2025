using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;

public class PlayerLookFPS_Terror_Normalized : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivity = 2.5f;
    public float stickSensitivity = 100f;

    [Header("Smoothing")]
    public float smoothTime = 0.05f;

    [Header("Head Bob")]
    public float bobFrequency = 1.5f;
    public float bobAmplitude = 0.03f;

    [Header("Idle sway")]
    public float swayAmount = 0.5f;
    public float swaySpeed = 0.5f;

    [Header("Referencias")]
    public Transform cameraHolder;   // pivote vertical de la cámara
    public Transform playerBody;     // pivote horizontal del jugador
    public Transform playerCamera;   // para head bob y sway

    [Header("Límites verticales")]
    public float verticalClamp = 80f;

    private PlayerInput input;
    private Vector2 lookInput;
    private Vector2 currentMouseDelta;
    private Vector2 smoothMouseVelocity;
    private float xRotation = 0f;

    private Vector3 initialCamPos;
    private float bobTimer = 0f;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        // Bloquear y ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
            initialCamPos = playerCamera.localPosition;
    }

    private void OnEnable()
    {
        input.actions["Look"].performed += OnLook;
        input.actions["Look"].canceled += OnLook;
    }

    private void OnDisable()
    {
        input.actions["Look"].performed -= OnLook;
        input.actions["Look"].canceled -= OnLook;
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (PlayerActionBlocker.Instance != null && PlayerActionBlocker.Instance.IsBlocked(PlayerAction.Look))
            return;

        HandleLook();
        HandleHeadBob();
        HandleIdleSway();
    }

    private void HandleLook()
    {
        // Detectar dispositivo
        bool isMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;
        bool isGamepad = Gamepad.current != null;

        Vector2 delta = lookInput;

        if (isMouse)
        {
            // Mouse ? usar input tal cual
            delta *= mouseSensitivity;
        }
        else if (isGamepad)
        {
            // Gamepad ? normalizar y multiplicar sensibilidad
            if (delta.sqrMagnitude > 1f) delta.Normalize();
            delta *= stickSensitivity * Time.deltaTime;
        }
        else
        {
            delta = Vector2.zero;
        }

        // Suavizado
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, delta, ref smoothMouseVelocity, smoothTime);

        float mouseX = currentMouseDelta.x;
        float mouseY = currentMouseDelta.y;

        // Vertical (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBob()
    {
        if (playerCamera == null) return;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null && cc.velocity.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 bobOffset = new Vector3(0f, Mathf.Sin(bobTimer) * bobAmplitude, 0f);
            playerCamera.localPosition = initialCamPos + bobOffset;
        }
        else
        {
            bobTimer = 0f;
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, initialCamPos, Time.deltaTime * 5f);
        }
    }

    private void HandleIdleSway()
    {
        if (playerCamera == null) return;

        float swayX = (Mathf.PerlinNoise(Time.time * swaySpeed, 0f) - 0.5f) * swayAmount;
        float swayY = (Mathf.PerlinNoise(0f, Time.time * swaySpeed) - 0.5f) * swayAmount;

        playerCamera.localRotation *= Quaternion.Euler(swayY, swayX, 0f);
    }
}
