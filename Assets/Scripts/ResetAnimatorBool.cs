using UnityEngine;

/// <summary>
/// This class is used to reset interaction variables to assist the animation into transitioning into other animations
/// </summary>
public class ResetAnimatorBool : StateMachineBehaviour
{
    public string targetBool;
    public bool status;

    /// <summary>
    /// Called when the state machine enters a state. Sets the specified boolean parameter 
    /// in the Animator to the defined status, allowing for controlled animation transitions.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { animator.SetBool(targetBool, status); }
}
