using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles third-person camera follow and rotation based on player input and target position.
    /// </summary>
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;

        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        private LayerMask ignoreLayers;

        public static CameraHandler singleton;

        [Header("Camera Settings")]
        public float lookSpeed = 0.1f;
        public float followSpeed = 0.15f;
        public float pivotSpeed = 0.03f;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        private float targetPosition;

        /// <summary>
        /// Locks or unlocks the cursor depending on input (mainly for editor testing).
        /// </summary>
        private void Start()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; }
            else { Cursor.lockState = CursorLockMode.Locked; }
        }

        /// <summary>
        /// Initializes singleton reference and camera-related data.
        /// </summary>
        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;

            // Ignore layers 8, 9, and 10 when casting (e.g., for occlusion checks)
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        /// <summary>
        /// Updates camera position smoothly every frame after all updates.
        /// </summary>
        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            FollowTarget(delta);
        }

        /// <summary>
        /// Smoothly follows the target position using damping for smooth camera motion.
        /// In addition to this, this also implements camera collision logic.
        /// </summary>
        /// <param name="delta">Time delta for smooth interpolation</param>
        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;
            HandleCameraCollision(delta);
        }

        /// <summary>
        /// Handles camera rotation based on mouse input. Rotates horizontally and tilts vertically.
        /// </summary>
        /// <param name="delta">Time delta for frame-rate independence</param>
        /// <param name="mouseXInput">Horizontal mouse movement</param>
        /// <param name="mouseYInput">Vertical mouse movement</param>
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            // Accumulate input for smooth rotation
            lookAngle += (mouseXInput * lookSpeed);
            pivotAngle -= (mouseYInput * pivotSpeed);

            // Clamp vertical rotation to prevent flipping
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            // Apply horizontal (yaw) rotation to the camera's root
            Quaternion targetRotation = Quaternion.Euler(0, lookAngle, 0);
            myTransform.rotation = targetRotation;

            // Apply vertical (pitch) rotation to the camera pivot
            Quaternion pivotRotation = Quaternion.Euler(pivotAngle, 0, 0);
            cameraPivotTransform.localRotation = pivotRotation;
        }

        /// <summary>
        /// Handles camera collision to prevent it from clipping through walls or objects.
        /// Uses a SphereCast from the camera pivot to detect obstacles between the pivot and the camera.
        /// Adjusts the camera's local Z position smoothly based on collision distance.
        /// </summary>
        /// <param name="delta">Delta time used for smooth interpolation of camera position.</param>
        public void HandleCameraCollision(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) { targetPosition -= minimumCollisionOffset; }
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
