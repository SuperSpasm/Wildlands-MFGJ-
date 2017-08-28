using UnityEngine;
using System.Collections;

public class PlayFXOnTransition : StateMachineBehaviour {
    private GameObject player;
    private SoundFXController playerSFXController;

    [Tooltip("if this state has an anystate / looping transition, the enter FX will play repeatedly. to work around this, the fx will only be played after this bool is false (and the state has been entered)")]
    public string resetWhenFalse;

    [Header("On Entering State")]
    public bool playFXOnEnter;
    [Tooltip("the position of the FX in the player SFXController's fx array")]
    public int enterFXIndex;

    [Header("On Exiting State")]
    public bool playFXOnExit;
    [Tooltip("the position of the FX in the player SFXController's fx array")]
    public int exitFXIndex;


    private bool setupComplete;                 // true when references have been set up
    private bool stateRepeating;                // true when state has been entered but not left yet

    void Setup()
    {
        // set up references. used to work around StateMachineBehaviors [presumably] executing before other scripts,
        // which means putting this in Awake() would result in a NullReferenceException for the player

        player = ScoutController.player;

        if (!player)
            throw new MissingReferenceException("can't find player!");
        else if (!player.GetComponent<SoundFXController>())
            throw new MissingComponentException("player required to have a SoundFXController!");
        else
        {
            playerSFXController = player.GetComponent<SoundFXController>();
            setupComplete = true;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!setupComplete)                     // if references have not yet been set up, do that now
            Setup();

        if (playFXOnEnter && !stateRepeating)
            playerSFXController.playFX(enterFXIndex);
        //Debug.Log("entered state. repeating? " + stateRepeating);
        stateRepeating = true;                  // make sure not to repeat this again until the state is exited and entered again
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (animator.GetBool(resetWhenFalse) == false)
            stateRepeating = false;             // set whether state is repeating or not

        if (playFXOnExit&& !stateRepeating)
            playerSFXController.playFX(exitFXIndex);


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
