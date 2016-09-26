using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class BreakBranch : MonoBehaviour {

    private enum BranchStatus {
                                CountingToBreak,                                    // Script enabled but branch has not been broken yet
                                StartedFalling,                                     // Branch has been broken, waiting to disable collider
                                PlayerStillAirBorne,                                   // Branch broken, collider disabled, waiting for player to be grounded
                                CountingToRespawn                                   // Branch broken, player was grounded, respawn counter in progress
                              }
    private BranchStatus branchStatus = BranchStatus.CountingToBreak;

	public float timeToFall;
	public float timeToReset = 2.0f;
	private Rigidbody2D rb;

	private float counter;
	[HideInInspector]public Vector3 initPos;                                         // set in BreakBranchController
	[HideInInspector]public Vector3 initRot;                                         // set in BreakBranchController
    private BreakBranchController brkBrnchCtr;

    [Header("SFX")]
    public AudioClip crackSFX;
    public AudioClip breakSFX;
    public AudioMixerGroup SFXMixer;
    public int cracksToSound = 1;
    private AudioSource audioSource;

    private Collider2D m_collider;
    private SpriteRenderer[] sprites;

    private float timeBetweenCracks;
    private float crackSfxTimer = 0;

    private Animator playerAnimator;                                                // used to determine whether player is grounded


    void Awake() {
        counter = 0.0f;
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = ScoutController.player.GetComponent<Animator>();
		brkBrnchCtr = GetComponent<BreakBranchController> ();

        timeBetweenCracks = timeToFall / (cracksToSound + 1);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixer;

        sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach(Collider2D coll in GetComponents<Collider2D>())
        { // get the non- trigger collider
            if (!coll.isTrigger)
            {
                if (m_collider) // make sure there's only one non-trigger collider
                    throw new UnityException(string.Format("There should only be one non-trigger collider on the breakable branch '{0}'! ", name));
                m_collider = coll;
            }
        }
        if (!m_collider)
            throw new System.ArgumentNullException(string.Format("no non-trigger collider2D found on breakable branch \"{ 0 }\"! ", name));
    }

	void Update () {

        counter += Time.deltaTime;                                                          // increase counter

        switch (branchStatus)
        {
            case BranchStatus.CountingToBreak:

                crackSfxTimer += Time.deltaTime;

                // if branch is still unbroken, and the time between cracks has passed
                if (crackSfxTimer >= timeBetweenCracks)
                {                                                                           // play a crack sound
                    crackSfxTimer = 0;
                    audioSource.clip = crackSFX;
                    audioSource.Play();
                }
                // else if the branch is still unbroken, and the time to fall has passed
                else if (counter >= timeToFall)
                {
                    Break();                                                                // break the branch
                    counter = 0.0f;                                                         // reset counter to count until collider disable;
                    branchStatus = BranchStatus.StartedFalling;                             // set status
                }
                break;

            case BranchStatus.StartedFalling:
                if (counter >= 0.2f)
                {
                    m_collider.enabled = false;                                            // only disable collider a small amount of time after breaking
                }                                                                          // this is so the branch falls in a more realistic manner
                branchStatus = BranchStatus.PlayerStillAirBorne;
                goto case BranchStatus.PlayerStillAirBorne;                                   // immediately go to the WaitingForGround case to detect a grounded player

            case BranchStatus.PlayerStillAirBorne:
                if (playerAnimator.GetBool("Ground") || playerAnimator.GetBool("Climb") || playerAnimator.GetBool("Swing"))                                      // player is grounded, climbing, or swinging
                {
                    counter = 0;                                                           // reset elapsed time to count until reset
                    branchStatus = BranchStatus.CountingToRespawn;                         // set status
                }
                break;

            case BranchStatus.CountingToRespawn:
                if (counter >= timeToReset)
                {
                    Reset();                                                               // if timeToReset has passed, reset the branch (also enables collider)
                }                                                                          // (Reset() also sets the status correctly)
                break;
        }
	}
    private void Break()
    {
        rb.gravityScale = 2.5f;
        rb.isKinematic = false;

        audioSource.clip = breakSFX;
        audioSource.Play();

        foreach (SpriteRenderer sprite in sprites)      // make branch sprites appear behind player
        { 
            sprite.sortingLayerName = "Ground_sort";    // set to ground layer so they're in front of nearly everything but behind player
            sprite.sortingOrder = 5;                    // bring them to front of layer to appear in front of ground
        }
    }
    private void Reset()
    {
        counter                      = 0.0f;     
        crackSfxTimer         = 0.0f;
        rb.gravityScale                  = 0.0f;
        rb.isKinematic                   = true;
        m_collider.enabled               = true;
        gameObject.transform.eulerAngles = initRot;
        gameObject.transform.position    = initPos;
        brkBrnchCtr.enabled              = true;
        branchStatus                     = BranchStatus.CountingToBreak;
        enabled                          = false;

        foreach (SpriteRenderer sprite in sprites)
        { // reset sprites in render order
            sprite.sortingLayerName = "Branches_sort";
            sprite.sortingOrder = 0;
        }
    }

}
