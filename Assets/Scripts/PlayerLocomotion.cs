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

        Vector3 moveDirection;
        Vector3 normalVector = Vector3.up; 

        public Rigidbody rigidBody;
        public GameObject normalCamera;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        [Header("Movement Statistics")]
        [SerializeField] float movementSpeed = 5f;     // Player's movement speed
        [SerializeField] float rotationSpeed = 10f;    // Speed at which the player rotates toward input direction
        [SerializeField] float sprintSpeed = 7f;       // Speed at which the player sprints at

        

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
            Vector3 smoothedDirection = Vector3.SmoothDamp(myTransform.forward, targetDirection, ref currentVelocity, 1f / rotationSpeed);

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
        /// Performs a backward roll (dodge) by applying velocity in the backward direction
        /// for the specified duration and speed. Stops player movement after completion.
        /// </summary>
        /// <param name="speed">The speed at which the player rolls backward.</param>
        /// <param name="duration">The duration of the roll in seconds.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        public IEnumerator PerformRollback(float speed, float duration)
        {
            float timer = 0f;
            Vector3 backwardDirection = -myTransform.forward;

            while (timer < duration)
            {
                rigidBody.linearVelocity = backwardDirection * speed;
                timer += Time.deltaTime;
                yield return null;
            }
            rigidBody.linearVelocity = Vector3.zero; // Stop motion after rollback
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
                    animatorHandler.PlayTargetAnimation("RollForward", true);
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    StartCoroutine(PerformRollForward(speed: 6f, duration: 0.6f));
                    playerInputHandler.rollFlag = false; // reset after consuming input
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("RollBackward", true);
                    Vector3 backwardDirection = -myTransform.forward;
                    backwardDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(backwardDirection);
                    myTransform.rotation = rollRotation;
                    StartCoroutine(PerformRollback(speed: 6f, duration: 0.6f));
                    playerInputHandler.rollFlag = false; // reset after consuming input
                }
            }
        }
    }
}