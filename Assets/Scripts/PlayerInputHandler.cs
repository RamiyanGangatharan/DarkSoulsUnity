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

        PlayerControls inputActions;

        Vector2 movementInput;
        Vector2 cameraInput;

        // Flags for roll and sprint state
        public bool b_Input;
        public bool rollFlag;
        public bool sprintFlag;

        // Reference to PlayerManager
        public PlayerManager playerManager;

        // Roll input timer
        private float rollInputTimer;

        /// <summary>
        /// This function makes sure that input is being detected
        /// </summary>
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                // Movement input
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                // Roll input
                inputActions.PlayerActions.Roll.started += ctx => OnRollInputStart();
                inputActions.PlayerActions.Roll.canceled += ctx => OnRollInputEnd();
            }

            inputActions.Enable();
        }

        private void OnDisable() { inputActions.Disable(); }

        /// <summary>
        /// This calls movement and control functions every physics tick
        /// </summary>
        public void TickInput(float delta)
        {
            MoveInput();
            HandleRollInput(delta);
        }

        /// <summary>
        /// This converts a clamped mouse input to a visual rotation of the camera
        /// </summary>
        private void MoveInput()
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        /// <summary>
        /// Handles roll input and sets flags for sprinting or rolling based on duration of input.
        /// </summary>
        private void HandleRollInput(float delta)
        {
            if (b_Input)
            {
                rollInputTimer += delta; // Increment roll timer when the button is held
                if (rollInputTimer >= 0.5f) { sprintFlag = true; rollFlag = false; }
            }
            else
            {
                // Button released, check if it was a short tap
                if (rollInputTimer > 0f && rollInputTimer < 0.5f) { rollFlag = true; sprintFlag = false; }
                rollInputTimer = 0f;
            }
        }

        private void OnRollInputStart() { b_Input = true; rollFlag = false;}
        private void OnRollInputEnd() { b_Input = false; }
    }
}
