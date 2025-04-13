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

        int vertical;
        int horizontal;

        public bool canRotate;

        /// <summary>
        /// Initializes the Animator component and hashes the parameter names for vertical and horizontal movements.
        /// </summary>
        public void Initialize()
        {
            animator = GetComponent<Animator>();

            // Convert parameter names to hash values for optimized performance.
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        /// <summary>
        /// Updates the animator values for vertical and horizontal movement.
        /// Snaps the movement values to predefined ranges (0, 0.5, -0.5, 1, -1) and applies them to the animator.
        /// </summary>
        /// <param name="verticalMovement">The vertical movement value (e.g., forward/backward).</param>
        /// <param name="horizontalMovement">The horizontal movement value (e.g., left/right).</param>
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
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

            // Updates the animator with the snapped vertical and horizontal values.
            // The third parameter (0.1f) is the transition speed, and Time.deltaTime ensures smooth animation.
            animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Allows the character to rotate by setting the canRotate flag to true.
        /// </summary>
        public void CanRotate() { canRotate = true; }

        /// <summary>
        /// Prevents the character from rotating by setting the canRotate flag to false.
        /// </summary>
        public void StopRotation() { canRotate = false; }
    }
}
