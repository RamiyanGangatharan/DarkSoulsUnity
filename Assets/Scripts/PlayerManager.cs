using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// This class manages the player's state of motion
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        PlayerInputHandler inputHandler;
        Animator animator;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        private Camera mainCamera;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isRolling;
        public bool isAerial;
        public bool isGrounded;

        public float rollInputTimer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            inputHandler = GetComponent<PlayerInputHandler>(); // Ensure this is assigned correctly
            if (inputHandler == null) { Debug.LogError("PlayerInputHandler is not attached to the player GameObject!"); }

            animator = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();

            playerLocomotion.animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerLocomotion.animatorHandler.playerManager = this;
        }

        /// <summary>
        /// Camera Initialization
        /// </summary>
        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            if (cameraHandler == null) { Debug.LogError("CameraHandler singleton is not assigned. Ensure CameraHandler is properly initialized."); }
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (inputHandler != null)
            {
                float delta = Time.deltaTime;

                inputHandler.TickInput(delta);

                playerLocomotion.HandleMovementInput(delta);
                playerLocomotion.HandleRollingandSprinting(delta);
                playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
                playerLocomotion.animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);

                if (playerLocomotion.animatorHandler.canRotate) { playerLocomotion.HandleRotation(delta); }

                isInteracting = animator.GetBool("isInteracting");
                isRolling = inputHandler.rollFlag;
            }
        }

        /// <summary>
        /// Calls the camera handling functions every frame
        /// </summary>
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            // Ensure cameraHandler is not null before using it
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
            else { Debug.LogError("CameraHandler is null in FixedUpdate."); }
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                inputHandler.rollFlag = false;
                inputHandler.sprintFlag = false;
                inputHandler.rightHandLightAttackInput = false;
                inputHandler.rightHandHeavyAttackInput = false;
               
                if (isAerial) { playerLocomotion.AirTimer = playerLocomotion.AirTimer + Time.deltaTime; }
            }
            else { Debug.LogError("CameraHandler is null in LateUpdate."); }
        }
    }
}