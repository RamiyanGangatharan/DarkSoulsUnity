using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("Camera Settings")]
        public float lookSpeed = 0.1f;
        public float followSpeed = 0.15f;
        public float pivotSpeed = 0.03f;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;

        private void Start()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; }
            else { Cursor.lockState = CursorLockMode.Locked; }
        }

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            FollowTarget(delta);
        }


        public void FollowTarget(float delta)
        {
            // Smoothly follow the target using SmoothDamp
            myTransform.position = Vector3.SmoothDamp(
                myTransform.position,
                targetTransform.position,
                ref cameraFollowVelocity,
                followSpeed
            );
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed);
            pivotAngle -= (mouseYInput * pivotSpeed);
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            // Horizontal camera rotation (yaw)
            Quaternion targetRotation = Quaternion.Euler(0, lookAngle, 0);
            myTransform.rotation = targetRotation;

            // Vertical camera tilt (pitch)
            Quaternion pivotRotation = Quaternion.Euler(pivotAngle, 0, 0);
            cameraPivotTransform.localRotation = pivotRotation;
        }
    }
}
