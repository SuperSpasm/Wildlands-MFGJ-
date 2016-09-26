using UnityEngine;
using System.Collections;

public class RandomizeStateTransition : StateMachineBehaviour {
    // randomizes transitions between states.
    // if ChanceToSendTrigger mode enabled - every [timeBetweenAttempts] there will be a [chance] chance to send the [trigger] to the animator
    // if ChanceToRepeatState mode enabled - once (on entering the state) there will be a [chance] chance to send a [repeatTrigger]
    // NOTE: the "repeat" trigger is a constant, and should be configured properly in the animator

    public enum Mode { ChanceToSendTrigger, ChanceToRepeatState }
    public Mode mode;

    [Range(0.0f, 1.0f)]
    public float chance = 0.5f;

    [Header("Trigger (non-repeat-mode) options")]
    public float timeBetweenAttempts = 1.0f;
    public string trigger;
    private float timeSinceLastAttempt = 0;

    [Header("Repeat-mode Settings")]
    public string repeatBool = "repeat";

    void Awake()
    {
        if (timeBetweenAttempts <= 0)
            throw new System.ArgumentException();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (mode == Mode.ChanceToRepeatState)
        {
            AttemptAction(animator);
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (mode == Mode.ChanceToSendTrigger)
        {
            timeSinceLastAttempt += Time.deltaTime;
            if (timeSinceLastAttempt >= timeBetweenAttempts)
            {
                AttemptAction(animator);
                timeSinceLastAttempt -= timeBetweenAttempts;
            }
        }
    }

    private void AttemptAction(Animator animator)
    {
        bool didSucceed = ( chance >= Random.value && chance != 0.0f );

        switch (mode)
            {
                case Mode.ChanceToRepeatState:
                    animator.SetBool(repeatBool, didSucceed);           // set bool to true if succeeded and false otherwise
                    break;
                case Mode.ChanceToSendTrigger:
                    if (didSucceed)
                        animator.SetTrigger(trigger);                   // set trigger if succeeded
                    break;
            }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
