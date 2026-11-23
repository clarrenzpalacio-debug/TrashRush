using UnityEngine;

namespace TrashRush.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 6.0f;
        [SerializeField] private float sprintSpeed = 9.5f;
        [SerializeField] private float acceleration = 18.0f;
        [SerializeField] private float deceleration = 24.0f;
        [SerializeField] private float rotationSpeed = 14.0f;

        [Header("Physics & Grounding")]
        [SerializeField] private float gravity = -20.0f;
        [SerializeField] private float groundStickForce = -3.5f;

        [Header("Animation")]
        [SerializeField] private PlayerAnimator playerAnimator;

        // Internal State
        private CharacterController controller;
        private Camera mainCamera;
        private PlayerStamina playerStamina;
        private Vector3 currentVelocity = Vector3.zero;
        private float verticalVelocity = -3.5f;
        private bool isSprinting = false;

        public float CurrentSpeed => new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        public bool IsGrounded => controller != null && controller.isGrounded;
        public bool IsSprinting => isSprinting;

        /// <summary>Access the player's stamina component.</summary>
        public PlayerStamina Stamina => playerStamina;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            if (playerAnimator == null)
            {
                playerAnimator = GetComponentInChildren<PlayerAnimator>();
            }

            // Attach or find the PlayerStamina component
            playerStamina = GetComponent<PlayerStamina>();
            if (playerStamina == null)
            {
                playerStamina = gameObject.AddComponent<PlayerStamina>();
            }
        }

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            HandleMovement();

            if (playerAnimator != null)
            {
                playerAnimator.UpdateAnimation(CurrentSpeed, isSprinting, controller.isGrounded);
            }
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 rawInput = new Vector3(h, 0f, v);
            Vector3 targetDirection = Vector3.zero;

            if (rawInput.sqrMagnitude > 0.01f)
            {
                rawInput.Normalize();

                // Compute camera-relative movement direction strictly on the horizontal XZ plane
                Vector3 camForward = (mainCamera != null) ? mainCamera.transform.forward : Vector3.forward;
                Vector3 camRight = (mainCamera != null) ? mainCamera.transform.right : Vector3.right;

                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                targetDirection = (camForward * rawInput.z + camRight * rawInput.x).normalized;

                // Smoothly rotate character toward movement heading
                if (targetDirection.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(targetDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }

            // Target speed — sprint only when Shift is held AND stamina allows
            bool wantsSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool isMoving = targetDirection.sqrMagnitude > 0.01f;
            isSprinting = wantsSprint && isMoving && playerStamina != null && playerStamina.CanSprint;

            // Notify stamina system and tick it
            if (playerStamina != null)
            {
                playerStamina.SetSprinting(isSprinting);
                playerStamina.Tick();
            }

            float targetSpeed = isMoving ? (isSprinting ? sprintSpeed : walkSpeed) : 0f;
            Vector3 targetVelocity = targetDirection * targetSpeed;

            // Acceleration / Deceleration
            float accelRate = (targetSpeed > 0.01f) ? acceleration : deceleration;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accelRate * Time.deltaTime);

            // Grounding & Gravity
            if (controller.isGrounded)
            {
                verticalVelocity = groundStickForce;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
                if (verticalVelocity < -25f) verticalVelocity = -25f;
            }

            // Combine horizontal velocity and vertical gravity
            Vector3 finalMove = currentVelocity;
            finalMove.y = verticalVelocity;

            controller.Move(finalMove * Time.deltaTime);

            // Safety floor clamp: under no condition should player descend below ground
            if (transform.position.y < 0f)
            {
                Vector3 p = transform.position;
                p.y = 0.01f;
                transform.position = p;
                verticalVelocity = groundStickForce;
            }
        }
    }
}
