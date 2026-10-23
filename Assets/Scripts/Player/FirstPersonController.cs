using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float gravity = -20f;
    [SerializeField, Range(0f, 90f)] private float maxLookAngle = 85f;
    [SerializeField] private float groundedDownSpeed = -2f;

    [Header("Start")]
    [SerializeField] private float lookDelayOnStart = 0.25f;

    public float SpeedMultiplier = 1f;
    public float CameraDrop;

    private CharacterController controller;
    private float pitch;
    private float verticalVelocity;
    private float cameraBaseHeight;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraRoot == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();
            if (childCamera != null) cameraRoot = childCamera.transform;
        }

        if (cameraRoot != null) cameraBaseHeight = cameraRoot.localPosition.y;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateCameraDrop();

        if (PhaseManager.IsPlayerLocked) return;

        if (MinigameManager.IsPhoneOpen)
        {
            Move();
            return;
        }
        if (MinigameManager.IsActive) return;

        Look();
        Move();
    }

    private void Look()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        if (Time.timeSinceLevelLoad < lookDelayOnStart) return;

        Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;

        transform.Rotate(0f, delta.x, 0f);
        pitch = Mathf.Clamp(pitch - delta.y, -maxLookAngle, maxLookAngle);

        if (cameraRoot != null)
        {
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void UpdateCameraDrop()
    {
        if (cameraRoot == null) return;

        Vector3 position = cameraRoot.localPosition;
        float target = cameraBaseHeight - CameraDrop;
        position.y = Mathf.MoveTowards(position.y, target, GameSettings.Current.potholeDropSpeed * Time.deltaTime);
        cameraRoot.localPosition = position;
    }

    private void Move()
    {
        float x = 0f;
        float z = 0f;

        GameSettings settings = GameSettings.Current;
        if (MinigameInput.KeyHeld(settings.moveRightKey)) x += 1f;
        if (MinigameInput.KeyHeld(settings.moveLeftKey)) x -= 1f;
        if (MinigameInput.KeyHeld(settings.moveForwardKey)) z += 1f;
        if (MinigameInput.KeyHeld(settings.moveBackKey)) z -= 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.sqrMagnitude > 1f) move.Normalize();

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = groundedDownSpeed;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed * SpeedMultiplier;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
