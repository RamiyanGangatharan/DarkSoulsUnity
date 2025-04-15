using UnityEngine;

namespace DarkSouls
{
    /// <summary>
    /// Handles animator parameters related to movement in the game.
    /// Updates the animator with snapped movement values for vertical and horizontal directions,
    /// and controls the character's ability to rotate.
    /// </summary>
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator animator;
        public PlayerInputHandler playerInputHandler;
        public PlayerLocomotion playerLocomotion;

        int vertical;
        int horizontal;

        public bool canRotate;

        /// <summary>
        /// Initializes the Animator component and hashes the parameter names for vertical and horizontal movements.
        /// </summary>
        public void Initialize()
        {
            animator = GetComponent<Animator>();
            playerInputHandler = GetComponentInParent<PlayerInputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        /// <summary>
        /// Updates the animator parameters for vertical and horizontal movement.
        /// Movement values are snapped to predefined steps (0, ±0.5, ±1) to ensure consistent transitions in animations.
        /// If the player is sprinting, vertical movement is set to 2 and horizontal remains analog for blend tree support.
        /// </summary>
        /// <param name="verticalMovement">Forward/backward input intensity (-1 to 1).</param>
        /// <param name="horizontalMovement">Left/right input intensity (-1 to 1).</param>
        /// <param name="isSprinting">Whether the player is currently sprinting (affects animation behavior).</param>

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            if (animator.GetBool("isInteracting")) return;

            // Default values to snap movements to predefined values.
            float snappedVertical = 0;
            float snappedHorizontal = 0;

            // Snaps the vertical movement value to predefined ranges.
            if (verticalMovement > 0 && verticalMovement < 0.55f) { snappedVertical = 0.5f; }
            else if (verticalMovement > 0.55f) { snappedVertical = 1; }
            else if (verticalMovement < 0 && verticalMovement > -0.55f) { snappedVertical = -0.5f; }
            else if (verticalMovement < -0.55f) { snappedVertical = -1; }

            // Snaps the horizontal movement value to predefined ranges.
            if (horizontalMovement > 0 && horizontalMovement < 0.55f) { snappedHorizontal = 0.5f; }
            else if (horizontalMovement > 0.55f) { snappedHorizontal = 1; }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f) { snappedHorizontal = -0.5f; }
            else if (horizontalMovement < -0.55f) { snappedHorizontal = -1; }
            else { snappedHorizontal = 0; }


            if (isSprinting)
            {
                snappedVertical = 2;
                snappedHorizontal = horizontalMovement;
            }

            // Updates the animator with the snapped vertical and horizontal values.
            // The third parameter (0.1f) is the transition speed, and Time.deltaTime ensures smooth animation.
            animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Plays the specified target animation using a smooth crossfade and sets interaction-related animation state.
        /// Also enables or disables root motion depending on whether the player is interacting (e.g., rolling, attacking).
        /// </summary>
        /// <param name="targetAnimation">The name of the animation state to transition to.</param>
        /// <param name="isInteracting">True if the player is performing an interaction that should affect movement (e.g., roll, attack); otherwise false.</param>
        public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnimation, 0.2f);
        }

        /// <summary>
        /// This implements movement from the animation to the rigidbody
        /// </summary>
        private void OnAnimatorMove()
        {
            if (playerInputHandler.isInteracting == false) { return; }

            float delta = Time.deltaTime;
            playerLocomotion.rigidBody.linearDamping = 0;

            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;

            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidBody.linearVelocity = velocity;

        }

        public void OnRollAnimationEnd() { animator.SetBool("isInteracting", false); }
        public void CanRotate() { canRotate = true; }
        public void StopRotation() { canRotate = false; }
    }
}
