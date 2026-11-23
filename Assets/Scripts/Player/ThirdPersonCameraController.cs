using UnityEngine;

namespace TrashRush.Player
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target Tracking")]
        [Tooltip("Transform of the player character to follow.")]
        [SerializeField] private Transform target;

        [Tooltip("Look target height offset above the player root (typically chest/shoulder level).")]
        [SerializeField] private float targetHeight = 1.6f;

        [Header("Camera Distance & Positioning")]
        [Tooltip("Ideal third-person distance behind the player.")]
        [SerializeField] private float defaultDistance = 5.0f;

        [Tooltip("Minimum allowable distance when colliding or zooming in.")]
        [SerializeField] private float minDistance = 1.0f;

        [Tooltip("Maximum allowable camera distance.")]
        [SerializeField] private float maxDistance = 7.5f;

        [Header("Orbit & Sensitivity")]
        [Tooltip("Horizontal orbit sensitivity.")]
        [SerializeField] private float mouseSensitivityX = 3.0f;

        [Tooltip("Vertical pitch sensitivity.")]
        [SerializeField] private float mouseSensitivityY = 2.5f;

        [Tooltip("Invert Y mouse axis.")]
        [SerializeField] private bool invertY = false;

        [Tooltip("Lowest vertical look angle (looking up at player).")]
        [SerializeField] private float minPitch = -15.0f;

        [Tooltip("Highest vertical look angle (looking down from above).")]
        [SerializeField] private float maxPitch = 65.0f;

        [Header("Damping & Responsiveness")]
        [Tooltip("Position smoothing speed.")]
        [SerializeField] private float positionDamping = 12.0f;

        [Tooltip("Rotation smoothing speed.")]
        [SerializeField] private float rotationDamping = 18.0f;

        [Header("Collision Handling (Anti-Clipping)")]
        [Tooltip("Layer mask containing environment objects to avoid clipping into.")]
        [SerializeField] private LayerMask obstacleLayers = ~0;

        [Tooltip("Safety sphere radius used to check camera collision against obstacles.")]
        [SerializeField] private float collisionRadius = 0.25f;

        [Tooltip("Offset away from collision surface to prevent near-clip plane clipping.")]
        [SerializeField] private float collisionOffset = 0.15f;

        // Internal Orbit Angles & Distance
        private float currentYaw = 0f;
        private float currentPitch = 18f;
        private float targetDistance = 5.0f;
        private float currentDistance = 5.0f;

        private void Start()
        {
            if (target != null)
            {
                currentYaw = target.eulerAngles.y;
            }

            targetDistance = defaultDistance;
            currentDistance = defaultDistance;

            // Unlock cursor initially so UI is accessible, but allow camera control
            Cursor.lockState = CursorLockMode.None;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleInput();
            HandleCollisionAndPositioning();
        }

        private void HandleInput()
        {
            // Allow continuous mouse look while right-clicking, holding Alt, or whenever cursor is locked
            bool isLooking = Input.GetMouseButton(1) || Input.GetKey(KeyCode.LeftAlt) || Cursor.lockState == CursorLockMode.Locked;

            // Optional mouse lock toggle with RMB
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (isLooking)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * (invertY ? 1f : -1f);

                currentYaw += mouseX;
                currentPitch -= mouseY;
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            }

            // Scroll wheel zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetDistance = Mathf.Clamp(targetDistance - scroll * 3.5f, minDistance, maxDistance);
            }
        }

        private void HandleCollisionAndPositioning()
        {
            Vector3 focusPoint = target.position + Vector3.up * targetHeight;
            Quaternion targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

            // Compute ideal unoccluded camera position
            Vector3 backDirection = targetRotation * Vector3.back;
            Vector3 idealPosition = focusPoint + backDirection * targetDistance;

            // Spherecast from focusPoint to idealPosition to detect obstacle clipping
            float desiredDistance = targetDistance;
            Ray ray = new Ray(focusPoint, backDirection);

            // Ignore Player and Trash layers during collision check
            int mask = obstacleLayers;
            int playerLayer = target.gameObject.layer;
            mask &= ~(1 << playerLayer);

            if (Physics.SphereCast(ray, collisionRadius, out RaycastHit hit, targetDistance, mask, QueryTriggerInteraction.Ignore))
            {
                // Found an obstacle between player focus point and camera
                desiredDistance = Mathf.Clamp(hit.distance - collisionOffset, minDistance, targetDistance);
            }

            // Smooth distance transition (fast zoom-in when hitting obstacle, smooth pull-back)
            float distanceDamping = (desiredDistance < currentDistance) ? 25.0f : 8.0f;
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * distanceDamping);

            Vector3 finalPosition = focusPoint + backDirection * currentDistance;

            // Apply position and look orientation smoothly
            transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * positionDamping);
            
            Quaternion lookRotation = Quaternion.LookRotation((focusPoint - transform.position).normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationDamping);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                currentYaw = target.eulerAngles.y;
            }
        }

        public void SetSensitivity(float sensX, float sensY)
        {
            mouseSensitivityX = sensX;
            mouseSensitivityY = sensY;
        }
    }
}
