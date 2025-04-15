using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player input for movement and camera control in the game.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        public float rollInputTimer;

        PlayerControls inputActions;
        CameraHandler cameraHandler;
        private Camera mainCamera;

        Vector2 movementInput;
        Vector2 cameraInput;

        public bool b_Input;
        public bool rollFlag;
        public bool sprintFlag;
        public bool isInteracting;



        /// <summary>
        /// Camera Initialization
        /// </summary>
        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            mainCamera = Camera.main; // assign the main camera here
        }

        /// <summary>
        /// Calls the camera handling functions every frame
        /// </summary>
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }


        /// <summary>
        /// This function makes sure that input is being detected
        /// </summary>
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                inputActions.PlayerActions.Roll.started += ctx => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += ctx => b_Input = false;

            }
            inputActions.Enable();
        }

        private void OnDisable() { inputActions.Disable(); }

        /// <summary>
        /// This calls movement and control functions every physics tick
        /// </summary>
        /// <param name="delta"></param>
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }

        /// <summary>
        /// This converts a clamped mouse input to a visual rotation of the camera
        /// </summary>
        /// <param name="delta"></param>
        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        /// <summary>
        /// Handles input logic for rolling and sprinting based on how long the roll button is held.
        /// A short tap triggers a roll, while holding the button for 0.5 seconds or longer triggers sprinting.
        /// </summary>
        /// <param name="delta">Time passed since the last frame (used to measure input hold duration).</param>

        private void HandleRollInput(float delta)
        {
            var rollInput = inputActions.PlayerActions.Roll;

            if (rollInput.triggered) { rollInputTimer = 0f; } // Detect initial button press

            // Check if the button is being held
            if (b_Input)
            {
                rollInputTimer += delta;
                if (rollInputTimer >= 0.5f) { sprintFlag = true; }
            }
            else
            {
                // Button released: check if it was a short tap for roll
                if (rollInputTimer > 0f && rollInputTimer < 0.5f)
                {
                    rollFlag = true;
                    sprintFlag = false;
                }
                rollInputTimer = 0f;
            }
        }
    }
}
