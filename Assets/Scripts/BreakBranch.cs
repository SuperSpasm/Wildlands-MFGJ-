using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class BreakBranch : MonoBehaviour {

	public float timeUntilFall = 2.5f;                                              // the time between a player stepping on the branch, and when it falls
	public float timeToReset = 2.0f;                                                // the time between a branch breaking, and resetting
	private Rigidbody2D rb;

	private Vector3 initPos;
	private Quaternion initRot;

    [Header("SFX")]
    public AudioClip crackSFX;
    public AudioClip breakSFX;

    public int cracksToSound = 1;
    private AudioSource audioSource;

    private Collider2D m_collider;
    private SpriteRenderer[] sprites;

    private float timeBetweenCracks;
    private float crackSfxTimer = 0;

    private Animator playerAnimator;                                                // used to determine whether player is grounded

    private bool triggered = false;                                                 // true after player steps on branch
    private bool broken = false;                                                    // is the branch currently broken?
    private bool canReset = false;                                                  // false if branch broke and player has been airborneever since, true otherwise.

    void Awake() {

        #region Get refs/ set vars
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        playerAnimator = ScoutController.player.GetComponent<Animator>();

        initPos = transform.position;
        initRot = transform.rotation;

        timeBetweenCracks = timeUntilFall / (cracksToSound + 1);
        #endregion

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
        bool waitingForGround = broken && !canReset;
        if (waitingForGround)
            if (playerAnimator.GetBool("Ground") || playerAnimator.GetBool("Climb") || playerAnimator.GetBool("Swing"))
                canReset = true;

        #region OLD
        //counter += Time.deltaTime;                                                          // increase counter

        //switch (branchStatus)
        //{
        //    case BranchStatus.CountingToBreak:

        //        crackSfxTimer += Time.deltaTime;

        //        // if branch is still unbroken, and the time between cracks has passed
        //        if (crackSfxTimer >= timeBetweenCracks)
        //        {                                                                           // play a crack sound
        //            crackSfxTimer = 0;
        //            audioSource.clip = crackSFX;
        //            audioSource.Play();
        //        }
        //        // else if the branch is still unbroken, and the time to fall has passed
        //        else if (counter >= timeUntilFall)
        //        {
        //            Break();                                                                // break the branch
        //            counter = 0.0f;                                                         // reset counter to count until collider disable;
        //            branchStatus = BranchStatus.StartedFalling;                             // set status
        //        }
        //        break;

        //    case BranchStatus.StartedFalling:
        //        if (counter >= 0.2f)
        //        {
        //            m_collider.enabled = false;                                            // only disable collider a small amount of time after breaking
        //        }                                                                          // this is so the branch falls in a more realistic manner
        //        branchStatus = BranchStatus.PlayerStillAirBorne;
        //        goto case BranchStatus.PlayerStillAirBorne;                                   // immediately go to the WaitingForGround case to detect a grounded player

        //    case BranchStatus.PlayerStillAirBorne:
        //        if (playerAnimator.GetBool("Ground") || playerAnimator.GetBool("Climb") || playerAnimator.GetBool("Swing"))                                      // player is grounded, climbing, or swinging
        //        {
        //            counter = 0;                                                           // reset elapsed time to count until reset
        //            branchStatus = BranchStatus.CountingToRespawn;                         // set status
        //        }
        //        break;

        //    case BranchStatus.CountingToRespawn:
        //        if (counter >= timeToReset)
        //        {
        //            Reset();                                                               // if timeToReset has passed, reset the branch (also enables collider)
        //        }                                                                          // (Reset() also sets the status correctly)
        //        break;
        //}
        #endregion
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (triggered)
            return;
        if (otherCollider.tag == "player_tag")
        {
            triggered = true;
            StartCoroutine(CrackThenBreak());
            return;
        }
    }

    private IEnumerator CrackThenBreak()
    {
        audioSource.clip = crackSFX;
        for (int i = 0; i < cracksToSound; i++)
        {
            yield return new WaitForSeconds(timeBetweenCracks);
            print("playing branch crack");
            audioSource.Play();
        }
        yield return new WaitForSeconds(timeBetweenCracks);
        Break();

    }
    private void Break()
    {
        broken = true;

        rb.gravityScale = 2.5f;
        rb.isKinematic = false;

        audioSource.clip = breakSFX;
        audioSource.Play();

        foreach (SpriteRenderer sprite in sprites)      // make branch sprites appear behind player
        { 
            sprite.sortingLayerName = "Ground_sort";    // set to ground layer so they're in front of nearly everything but behind player
            sprite.sortingOrder = 5;                    // bring them to front of layer to appear in front of ground
        }
        StartCoroutine(DisableCollider(0.2f));
        StartCoroutine(Reset(timeToReset));


    }
    private IEnumerator DisableCollider(float wait)
    {
        yield return new WaitForSeconds(wait);
        m_collider.enabled = false;
    }
    private IEnumerator Reset(float wait)
    {
        yield return new WaitForSeconds(wait);
        yield return new WaitUntil(() => canReset == true);             // only reset if canReset flag is true
        
        rb.gravityScale                     = 0.0f;
        rb.velocity                         = Vector2.zero;
        rb.bodyType                         = RigidbodyType2D.Static;
        m_collider.enabled                  = true;
        transform.rotation                  = initRot;
        transform.position                  = initPos;

        broken                              = false;
        canReset                            = false;
        triggered                           = false;

        foreach (SpriteRenderer sprite in sprites)
        { // reset sprites in render order
            sprite.sortingLayerName = "Branches_sort";
            sprite.sortingOrder = 0;
        }
    }


}
