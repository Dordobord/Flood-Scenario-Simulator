using UnityEngine;
using UnityEngine.InputSystem;

namespace StormWaits
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float lookSensitivity = 0.1f;
        [SerializeField] private float gravity = -20f;

        public float SpeedMultiplier = 1f;

        private CharacterController controller;
        private float pitch;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            
            if (cameraRoot == null)
            {
                Camera childCamera = GetComponentInChildren<Camera>();
                if (childCamera != null) cameraRoot = childCamera.transform;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (MinigameManager.IsActive || PhaseManager.IsTransitionActive) return;

            Look();
            Move();
        }

        private void Look()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;

            transform.Rotate(0f, delta.x, 0f);
            pitch = Mathf.Clamp(pitch - delta.y, -85f, 85f);

            if (cameraRoot != null)
            {
                cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        private void Move()
        {
            float x = 0f;
            float z = 0f;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.dKey.isPressed) x += 1f;
                if (keyboard.aKey.isPressed) x -= 1f;
                if (keyboard.wKey.isPressed) z += 1f;
                if (keyboard.sKey.isPressed) z -= 1f;
            }

            Vector3 move = transform.right * x + transform.forward * z;
            if (move.sqrMagnitude > 1f) move.Normalize();

            
            if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = move * moveSpeed * SpeedMultiplier;
            velocity.y = verticalVelocity;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
