using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles player movement, rotation, and animation based on input and camera orientation.
    /// </summary>
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        PlayerInputHandler playerInputHandler;

        Vector3 moveDirection;
        Vector3 normalVector = Vector3.up;

        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public Rigidbody rigidBody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField] float movementSpeed = 5f;
        [SerializeField] float rotationSpeed = 10f;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        private void Update()
        {
            float delta = Time.deltaTime;

            playerInputHandler.TickInput(delta);
            HandleMovementInput(delta);
            animatorHandler.UpdateAnimatorValues(playerInputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void HandleMovementInput(float delta)
        {
            moveDirection = cameraObject.forward * playerInputHandler.vertical;
            moveDirection += cameraObject.right * playerInputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection *= movementSpeed;
        }

        private void Move()
        {
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidBody.linearVelocity = projectedVelocity;
        }

        private void HandleRotation(float delta)
        {
            Vector3 targetDirection = cameraObject.forward * playerInputHandler.vertical;
            targetDirection += cameraObject.right * playerInputHandler.horizontal;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = myTransform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotationSpeed * delta);
            myTransform.rotation = smoothedRotation;
        }
    }
}
