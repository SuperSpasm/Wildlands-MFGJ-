using UnityEngine;
using System.Collections;

public class PauseClimb : StateMachineBehaviour {
    private float initialSpeed;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        initialSpeed = animator.speed; // get the intial speed of the animator. (likely 1)
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!animator.GetBool("Climb"))      // if player is no longer climbing, ensure normal playback speed.
            animator.speed = initialSpeed;
        else if (animator.GetFloat("hSpeed") == 0 && animator.GetFloat("vSpeed") == 0) // if the player is climbing and not moving in either axis, pause the animator
            animator.speed = 0;
        else                               // else ensure normal playback speed.
            animator.speed = initialSpeed;
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.speed = initialSpeed; // exiting state, return to normal speed
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
