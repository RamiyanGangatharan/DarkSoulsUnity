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
        CameraHandler cameraHandler;
        private Camera mainCamera;

        Vector2 movementInput;
        Vector2 cameraInput;

        public bool b_Input;
        public bool rollFlag;
        public bool isInteracting;

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            mainCamera = Camera.main; // assign the main camera here
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
        }


        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            var rollInput = inputActions.PlayerActions.Roll;
            if (rollInput.triggered)
            {
                rollFlag = true;
            }
        }


    }
}
