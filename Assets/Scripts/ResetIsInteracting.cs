using UnityEngine;

/// <summary>
/// This class is used to reset interaction variables to assist the animation into transitioning into other animations
/// </summary>
public class ResetIsInteracting : StateMachineBehaviour
{
    /// <summary>
    /// Called when the animation state has finished playing. Sets the "isInteracting" parameter
    /// to false, allowing the player to resume movement or other actions once the animation ends.
    /// </summary>
    /// <param name="animator">The Animator component responsible for the animation.</param>
    /// <param name="stateInfo">Information about the current animation state.</param>
    /// <param name="layerIndex">The index of the animation layer being processed.</param>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { animator.SetBool("isInteracting", false); }
}
