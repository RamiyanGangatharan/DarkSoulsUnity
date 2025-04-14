using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player locomotion, rotation, and animation updates based on input and camera orientation.
    /// </summary>
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        PlayerInputHandler playerInputHandler;

        Vector3 moveDirection;
        Vector3 normalVector = Vector3.up; // Used to project movement onto a flat surface (like ground)

        public Rigidbody rigidBody;
        public GameObject normalCamera;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        [Header("Stats")]
        [SerializeField] float movementSpeed = 5f;     // Player's movement speed
        [SerializeField] float rotationSpeed = 10f;    // Speed at which the player rotates toward input direction

        /// <summary>
        /// Initializes components required for player control and animation.
        /// </summary>
        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        /// <summary>
        /// Processes input and updates rotation/animation each frame.
        /// </summary>
        private void Update()
        {
            float delta = Time.deltaTime;

            playerInputHandler.TickInput(delta); // Process player inputs
            HandleMovementInput(delta); // Calculate intended movement direction based on camera orientation
            animatorHandler.UpdateAnimatorValues(playerInputHandler.moveAmount, 0); // Update animations with current input
            if (animatorHandler.canRotate) { HandleRotation(delta); }
        }

        /// <summary>
        /// Applies movement to the Rigidbody each physics tick.
        /// </summary>
        private void FixedUpdate() { Move(); }

        /// <summary>
        /// Calculates the direction and magnitude of movement based on player input and camera.
        /// </summary>
        private void HandleMovementInput(float delta)
        {
            moveDirection = cameraObject.forward * playerInputHandler.vertical;
            moveDirection += cameraObject.right * playerInputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection *= movementSpeed;
        }

        /// <summary>
        /// Moves the player character using physics, respecting surface orientation.
        /// </summary>
        private void Move()
        {
            // Project the movement vector onto the plane defined by normalVector (typically the ground)
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidBody.linearVelocity = projectedVelocity;
        }

        // --------------------------------------------------------------------------------------

        private Vector3 currentVelocity = Vector3.zero; // Used by SmoothDamp to track velocity

        /// <summary>
        /// Rotates the player smoothly toward the direction of input relative to the camera.
        /// </summary>
        private void HandleRotation(float delta)
        {
            // Determine the direction the player should face based on input
            Vector3 targetDirection = cameraObject.forward * playerInputHandler.vertical;
            targetDirection += cameraObject.right * playerInputHandler.horizontal;
            targetDirection.Normalize();
            targetDirection.y = 0; // Ensure rotation happens only on the Y axis (horizontal)

            if (targetDirection == Vector3.zero) { targetDirection = myTransform.forward; }

            // Smoothly interpolate the forward direction to the target using SmoothDamp
            Vector3 smoothedDirection = Vector3.SmoothDamp(
                myTransform.forward,
                targetDirection,
                ref currentVelocity,
                1f / rotationSpeed // Inverse of speed gives smoothing time
            );

            // Apply the new rotation if the smoothed direction is valid
            if (smoothedDirection != Vector3.zero) { myTransform.rotation = Quaternion.LookRotation(smoothedDirection); }
        }
    }
}
