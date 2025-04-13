using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player movement, rotation, and animation based on input and camera orientation.
    /// </summary>
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("References")]
        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;

        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float rotationSpeed = 10f;

        private PlayerInputHandler playerInputHandler;
        private Transform cameraObject;
        private Vector3 normalVector = Vector3.up;

        /// <summary>
        /// Initializes references and prepares components at start.
        /// </summary>
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler?.Initialize();
        }

        /// <summary>
        /// Updates input and movement every frame.
        /// </summary>
        void Update()
        {
            float delta = Time.deltaTime;
            playerInputHandler.TickInput(delta);
            HandleMovement(delta);
            HandleAnimator();
            if (animatorHandler != null && animatorHandler.canRotate) { HandleRotation(delta); }
        }

        /// <summary>
        /// Handles movement logic relative to the camera direction.
        /// </summary>
        /// <param name="delta">The delta time for this frame.</param>
        void HandleMovement(float delta)
        {
            Vector3 input = new Vector3(playerInputHandler.horizontal, 0f, playerInputHandler.vertical);

            Vector3 camForward = cameraObject.forward;
            Vector3 camRight = cameraObject.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 movement = camForward * input.z + camRight * input.x;
            movement.Normalize();
            movement *= movementSpeed;

            Vector3 velocity = Vector3.ProjectOnPlane(movement, normalVector);
            rigidbody.MovePosition(rigidbody.position + velocity * delta);
        }

        /// <summary>
        /// Updates animator parameters based on input.
        /// </summary>
        void HandleAnimator() { animatorHandler?.UpdateAnimatorValues(playerInputHandler.moveAmount, 0); }

        /// <summary>
        /// Smoothly rotates the player to face the direction of input.
        /// </summary>
        /// <param name="delta">The delta time for this frame.</param>
        void HandleRotation(float delta)
        {
            Vector3 inputDir = new Vector3(playerInputHandler.horizontal, 0f, playerInputHandler.vertical);

            if (inputDir.sqrMagnitude == 0f) { return; }

            Vector3 camForward = cameraObject.forward;
            Vector3 camRight = cameraObject.right;

            camForward.y = 0;
            camRight.y = 0;

            Vector3 targetDir = camForward * inputDir.z + camRight * inputDir.x;

            if (targetDir.sqrMagnitude == 0f) { return; };

            Quaternion targetRotation = Quaternion.LookRotation(targetDir.normalized);
            Quaternion smoothedRotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotationSpeed * delta);

            myTransform.rotation = smoothedRotation;
        }
    }
}
