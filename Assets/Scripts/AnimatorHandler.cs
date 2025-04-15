using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles animator parameters related to movement and interaction.
    /// </summary>
    public class AnimatorHandler : MonoBehaviour
    {
        [HideInInspector] public Animator animator;
        [HideInInspector] public PlayerInputHandler playerInputHandler;
        [HideInInspector] public PlayerLocomotion playerLocomotion;
        [HideInInspector] public PlayerManager playerManager;

        private int vertical;
        private int horizontal;

        public bool canRotate;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            playerManager = GetComponentInParent<PlayerManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");

            if (!animator || !playerInputHandler || !playerLocomotion || !playerManager) { Debug.LogError("AnimatorHandler missing one or more required references."); }
        }

        /// <summary>
        /// Updates animator blend tree values.
        /// </summary>
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            if (animator.GetBool("isInteracting")) { return; }

            float snappedVertical = SnapInput(verticalMovement);
            float snappedHorizontal = SnapInput(horizontalMovement);

            if (isSprinting)
            {
                snappedVertical = 2f;
                snappedHorizontal = horizontalMovement;
            }

            animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        }

        private float SnapInput(float input)
        {
            if (input > 0.55f) { return 1f; }
            if (input > 0f) { return 0.5f; }
            if (input < -0.55f) { return -1f; }
            if (input < 0f) { return -0.5f; }
            else { return 0f; }
        }

        /// <summary>
        /// Plays an animation with root motion and crossfade.
        /// </summary>
        public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            if (animator == null) { return; }
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnimation, 0.2f);
        }

        /// <summary>
        /// This effectively attaches animations to a rigidbody properly
        /// </summary>
        private void OnAnimatorMove()
        {
            if (animator == null || playerManager == null || playerLocomotion == null) { return; }
            if (!playerManager.isInteracting) { return; }

            float delta = Time.deltaTime;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0f;

            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidBody.linearDamping = 0f;
            playerLocomotion.rigidBody.linearVelocity = velocity;
        }

        public void OnRollAnimationEnd() { if (animator != null) { animator.SetBool("isInteracting", false); } }
        public void CanRotate() => canRotate = true;
        public void StopRotation() => canRotate = false;
    }
}
