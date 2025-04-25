using System.Collections;
using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player locomotion, rotation, and animation updates based on input and camera orientation.
    /// </summary>
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        PlayerInputHandler playerInputHandler;

        public Vector3 moveDirection;
        Vector3 normalVector = Vector3.up;
        Vector3 targetPosition;

        public Rigidbody rigidBody;
        public GameObject normalCamera;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        [Header("Movement Statistics")]
        [SerializeField] float movementSpeed = 5f;     // Player's movement speed
        [SerializeField] float rotationSpeed = 10f;    // Speed at which the player rotates toward input direction
        [SerializeField] float sprintSpeed = 7f;       // Speed at which the player sprints at
        [SerializeField] float fallingSpeed = 45f;     // Speed at which the player falls from the air

        [Header("Aerial & Ground Detection Statistics")]
        [SerializeField] float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] float groundDirectionRayDistance = 0.2f;
        [SerializeField] float minimumMandatoryFallDistance = 1.0f;
        LayerMask ignoreForGroundCheck;
        public float AirTimer;


        /// <summary>
        /// Initializes components required for player control and animation.
        /// </summary>
        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidBody = GetComponent<Rigidbody>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

        /// <summary>
        /// Applies movement to the Rigidbody each physics tick.
        /// </summary>
        private void FixedUpdate() { Move(); }

        /// <summary>
        /// Calculates the direction and magnitude of movement based on player input and camera.
        /// </summary>
        public void HandleMovementInput(float delta)
        {
            if (playerInputHandler.rollFlag) { return; }
            if (playerManager.isInteracting) { return; }

            moveDirection = cameraObject.forward * playerInputHandler.vertical;
            moveDirection += cameraObject.right * playerInputHandler.horizontal;
            moveDirection.Normalize();

            float currentSpeed = playerInputHandler.sprintFlag ? sprintSpeed : movementSpeed;

            moveDirection *= currentSpeed;
            playerManager.isSprinting = playerInputHandler.sprintFlag;
        }


        /// <summary>
        /// Moves the player character using physics, respecting surface orientation.
        /// Projects the movement vector onto the plane defined by normalVector (typically the ground)
        /// </summary>
        private void Move()
        {
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidBody.linearVelocity = projectedVelocity;
        }

        private Vector3 currentVelocity = Vector3.zero; // Used by SmoothDamp to track velocity

        /// <summary>
        /// Rotates the player smoothly toward the direction of input relative to the camera.
        /// </summary>
        public void HandleRotation(float delta)
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
                1f / rotationSpeed
            );

            // Apply the new rotation if the smoothed direction is valid
            if (smoothedDirection != Vector3.zero) { myTransform.rotation = Quaternion.LookRotation(smoothedDirection); }
        }

        /// <summary>
        /// Performs a forward roll movement by applying velocity in the forward direction
        /// for the specified duration and speed. After rolling, resumes normal movement if input is detected.
        /// </summary>
        /// <param name="speed">The speed at which the player rolls forward.</param>
        /// <param name="duration">The duration of the roll in seconds.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        public IEnumerator PerformRollForward(float speed, float duration)
        {
            float timer = 0f;
            Vector3 forwardDirection = myTransform.forward;

            while (timer < duration)
            {
                rigidBody.linearVelocity = forwardDirection * speed;
                timer += Time.deltaTime;
                yield return new WaitForSeconds(0.1f);
            }

            // Allow movement after roll based on current input
            if (playerInputHandler.moveAmount > 0) { HandleMovementInput(playerInputHandler.moveAmount); }
            else { rigidBody.linearVelocity = Vector3.zero; }
        }

        /// <summary>
        /// Handles the logic for initiating rolling and sprinting based on player input.
        /// Plays appropriate animations and initiates forward or backward roll movement depending on direction.
        /// </summary>
        /// <param name="delta">Time passed since the last frame, used for time-based calculations.</param>
        public void HandleRollingandSprinting(float delta)
        {
            if (animatorHandler.animator.GetBool("isInteracting")) { return; }

            if (playerInputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * playerInputHandler.vertical;
                moveDirection += cameraObject.right * playerInputHandler.horizontal;
                moveDirection.y = 0;

                if (playerInputHandler.moveAmount > 0)
                {
                    animatorHandler.animator.CrossFade("RollForward", 0.1f);
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    StartCoroutine(PerformRollForward(speed: 6f, duration: 0.6f));
                    playerInputHandler.rollFlag = false; // reset after consuming input
                }
            }
        }

        /// <summary>
        /// Handles player falling logic by applying gravity and limited air control while airborne,
        /// detecting ground contact using raycasts, and triggering appropriate landing or falling animations.
        /// Transitions the player between grounded and aerial states based on raycast hits and airtime duration.
        /// </summary>
        /// <param name="delta">Time passed since the last frame (used to accumulate airtime).</param>
        /// <param name="moveDirection">The direction the player is attempting to move, used for in-air control and raycast orientation.</param>
        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f)) { moveDirection = Vector3.zero; }
            if (playerManager.isAerial)
            {
                rigidBody.AddForce(-Vector3.up * fallingSpeed);
                rigidBody.AddForce(moveDirection * fallingSpeed / 50f);
            }

            Vector3 direction = moveDirection;
            direction.Normalize();
            origin = origin + direction * groundDirectionRayDistance;
            targetPosition = myTransform.position;

            Debug.DrawRay(
                origin, 
                -Vector3.up * minimumMandatoryFallDistance, 
                Color.red, 
                0.1f, 
                false
            );

            if (Physics.Raycast(
                    origin, 
                    -Vector3.up, 
                    out hit, 
                    minimumMandatoryFallDistance, 
                    ignoreForGroundCheck
                    )
                )
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                targetPosition.y = tp.y;

                if (playerManager.isAerial)
                {
                    if (AirTimer > 0.5f) { animatorHandler.PlayTargetAnimation("RollForward", true); AirTimer = 0; }
                    else { animatorHandler.PlayTargetAnimation("Empty", false); AirTimer = 0; }
                    playerManager.isAerial = false;
                }
            }
            else
            {
                if (playerManager.isGrounded) { playerManager.isGrounded = false; }
                if (playerManager.isAerial == false)
                {
                    if (playerManager.isInteracting == false) { animatorHandler.PlayTargetAnimation("FallingLoop", true); }

                    Vector3 vel = rigidBody.linearVelocity;
                    vel.Normalize();

                    rigidBody.linearVelocity = vel * (movementSpeed / 2);
                    playerManager.isAerial = true;
                }
            }
            if (playerManager.isInteracting || playerInputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(
                    transform.position, 
                    targetPosition, 
                    Time.deltaTime
                );
            }
            else { myTransform.position = targetPosition; }
        }
    }
}
