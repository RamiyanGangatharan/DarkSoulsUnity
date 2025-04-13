using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player input for movement and camera control in the game.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Movement Input")]
        public float horizontal;
        public float vertical;

        [Header("Movement Input")]
        public float moveAmount;

        [Header("Camera Input")]
        public float mouseX;
        public float mouseY;

        private PlayerControls controls;
        CameraHandler cameraHandler;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        /// <summary>
        /// Initializes the player input controls and subscribes to input events.
        /// </summary>
        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;

            if (controls == null)
            {
                controls = new PlayerControls();
                controls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                controls.PlayerMovement.Camera.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// This function will control camera movements when operating the player
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
        /// Enables the input actions when the object is active.
        /// </summary>
        private void OnEnable() => controls.Enable();

        /// <summary>
        /// Disables the input actions when the object is inactive.
        /// </summary>
        private void OnDisable() => controls.Disable();

        /// <summary>
        /// Called externally every frame to handle player input logic.
        /// </summary>
        /// <param name="delta">The delta time for the current frame.</param>
        public void TickInput(float delta) { ProcessMovementInput(); } // Continuously update input values every frame.

        /// <summary>
        /// Processes movement and camera input to extract usable values for player movement and camera control.
        /// </summary>
        private void ProcessMovementInput()
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;

            // Calculate movement amount based on input.
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            // Capture mouse input for camera control.
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }
    }
}
