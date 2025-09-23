using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerActionBlocker;

public class PlayerLookFPS : MonoBehaviour
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
        HandleHeadBobAndSway();
    }

    private void HandleLook()
    {
        // Detectar dispositivo
        bool isMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;
        bool isGamepad = Gamepad.current != null;

        Vector2 delta = lookInput;

        if (isMouse)
        {
            delta *= mouseSensitivity;
        }
        else if (isGamepad)
        {
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

    private void HandleHeadBobAndSway()
    {
        if (playerCamera == null) return;

        // Base position y rotación
        Vector3 basePos = initialCamPos;
        Quaternion baseRot = Quaternion.Euler(xRotation, 0f, 0f);

        CharacterController cc = GetComponent<CharacterController>();
        bool isMoving = cc != null && cc.velocity.magnitude > 0.1f;
        bool isLooking = lookInput.magnitude > 0.01f;

        // --- Head Bob ---
        Vector3 bobOffset = Vector3.zero;
        if (isMoving)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            bobOffset = new Vector3(0f, Mathf.Sin(bobTimer) * bobAmplitude, 0f);
        }
        else
        {
            bobTimer = 0f;
        }

        // --- Idle Sway ---
        Vector3 swayEuler = Vector3.zero;
        if (!isMoving && !isLooking)
        {
            float swayX = (Mathf.PerlinNoise(Time.time * swaySpeed, 0f) - 0.5f) * swayAmount;
            float swayY = (Mathf.PerlinNoise(0f, Time.time * swaySpeed) - 0.5f) * swayAmount;
            swayEuler = new Vector3(swayY, swayX, 0f);
        }

        // --- Aplicar posición y rotación ---
        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, basePos + bobOffset, Time.deltaTime * 8f);
        Quaternion swayRotation = Quaternion.Euler(swayEuler);
        playerCamera.localRotation = Quaternion.Slerp(playerCamera.localRotation, baseRot * swayRotation, Time.deltaTime * 8f);
    }
}
