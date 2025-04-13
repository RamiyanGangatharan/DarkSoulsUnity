using UnityEngine;

namespace DarkSouls
{
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

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        public float minimumPivot = -35;
        public float maximumPivot = 35;
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private float defaultPosition;
        private float targetPosition;
        private float lookAngle;
        private float pivotAngle;


        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        void FixedUpdate()
        {
            float delta = Time.deltaTime;
            FollowTarget(delta);
        }

        public void FollowTarget(float delta)
        {
            float smoothSpeed = Mathf.Clamp(followSpeed, 0.01f, 1f);
            myTransform.position = Vector3.Lerp(myTransform.position, targetTransform.position, smoothSpeed);
            HandleCameraCollisions(delta);
        }

        /// <summary>
        /// Rotates the camera around the target based on mouse input.
        /// Horizontal mouse movement rotates the camera rig around the target (yaw),
        /// while vertical mouse movement tilts the camera up and down (pitch) using a pivot.
        /// Rotation angles are smoothed and clamped to prevent extreme angles.
        /// </summary>
        /// <param name="delta">Delta time for smoothing rotation updates.</param>
        /// <param name="mouseXInput">Horizontal mouse input (X-axis) for yaw rotation.</param>
        /// <param name="mouseYInput">Vertical mouse input (Y-axis) for pitch rotation.</param>

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            // Apply horizontal rotation to the main camera rig
            myTransform.rotation = Quaternion.Euler(0, lookAngle, 0);

            // Apply vertical rotation to the pivot (camera up/down)
            cameraPivotTransform.localRotation = Quaternion.Euler(pivotAngle, 0, 0);
        }


        /// <summary>
        /// Handles camera collisions by performing a sphere cast from the camera pivot to the camera, 
        /// adjusting the camera's local Z position to avoid clipping through geometry.
        /// Smoothly interpolates the camera position based on collision distance and delta time.
        /// </summary>
        /// <param name="delta">Delta time used for smoothing the camera movement.</param>

        private void HandleCameraCollisions(float delta)
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
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset) { targetPosition = -minimumCollisionOffset; }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}