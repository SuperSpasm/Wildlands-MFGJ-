using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoutController : MonoBehaviour
{

    [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

    private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Transform m_CeilingCheck; // A position marking where to check for ceilings
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

    private Animator m_Anim;            // Reference to the player's animator component.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private BoxCollider2D m_bodyCollider;
    private CircleCollider2D m_feetCollider;


    //
    //// CLIMBING
    //

    [HideInInspector] public GameObject climbingOnThis;    // null if not climbing on, otherwise has a reference to the object currently climbing on

    [HideInInspector] public GameObject availableForClimb; // will have a reference to a climbable object if you're in the vicinity of one. null otherwise.
    [SerializeField]  private float m_climbSpeed;
    private float m_gravityScaleDefault;                   // Since climbing disables gravity, keep track of original value
    private bool climbDisabled = false;                    // disable climbing for a short period after jumping

    [SerializeField]
    [Tooltip("after jumping from a climbable surface climbing will be disabled on that object for this short time")]
    private float climbDisableTime = 0.2f;
    private float climbDisableCounter = 0;
    private GameObject disabledClimbObject;

    private ClimbableTree treeScript;
    //
    //
    ////
    //
    //// SWINGING
    //
    
    //[HideInInspector]
    public GameObject swingingOnThis;          // null if not swinging, otherwise has a reference to the object currently climbing on
    
    //[HideInInspector]
    public List<GameObject> availableForSwing; // will have a reference to swingable objects if you're in the vicinity of some. empty otherwise.
    [SerializeField]  private float m_swingSpeed;
    
    [Tooltip("after jumping from a swingable object swinging on that object will be disabled for this short time")]
    [SerializeField] private float swingDisableTime = 0.2f;

    [SerializeField] private bool vineAutoAttach;
    //[SerializeField] private bool allowSlideDownVine;
    // Might be too much work to finish this, since id have to implement a better way to find the closest vine (started on that using raycasting)
    // and since i just lower swingJoint to slide down, id have to handle situations when i hit the ground. probably would be easier to design levels such that you dont need this.

    [SerializeField] private Vector2 swingPositionOffset;
    [SerializeField] private Vector2 swingRotationOffset;
    HingeJoint2D swingJoint;                                    // joint to be added to the player  attached to the vine while swinging
    private VineLink vineScript;


    private enum SwingDisableStatus { enabled, waitingForGroud, Disabled };
    private SwingDisableStatus swingDisabled = SwingDisableStatus.enabled;
    private float swingDisableCounter = 0;
    private GameObject disabledVineRoot;

	private SoundFXController sfxCtr;
    //
    //
    ////
    //

    private void Awake()
    {
        // Setting up references.
        m_GroundCheck = transform.Find("GroundCheck");
        m_CeilingCheck = transform.Find("CeilingCheck");
        m_Anim = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_bodyCollider = GetComponent<BoxCollider2D>();
        m_feetCollider = GetComponent<CircleCollider2D>();

        m_gravityScaleDefault = m_Rigidbody2D.gravityScale;

        availableForSwing = new List<GameObject>();
		sfxCtr = GetComponent<SoundFXController> ();
    }


    private void FixedUpdate()
    {
        GroundCheck();
        // Set the vertical, horizontal speeds for animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        m_Anim.SetFloat("hSpeed", m_Rigidbody2D.velocity.x);
    }

    public void GroundCheck()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                float colliderHeight = colliders[i].transform.position.y;
                float feetHeight = m_GroundCheck.position.y;

                // make sure the character is ABOVE the collider if it's not rotated
                // (don't perform this check for rotated objects, since it causes unexpected behavior)
                if (colliders[i].transform.rotation != Quaternion.identity
                  || feetHeight >= colliderHeight)
                    m_Grounded = true;

            }
        }
        m_Anim.SetBool("Ground", m_Grounded);
    }

    private void Update()
    {
        if (climbDisabled)
        { // keep track of whether climb is enabled
            if (climbDisableCounter >= climbDisableTime)
            {
                climbDisabled = false;
                climbDisableCounter = 0;
                disabledClimbObject = null;
            }
            climbDisableCounter += Time.deltaTime;
        }

        if (swingDisabled == SwingDisableStatus.waitingForGroud)
            if(m_Anim.GetBool("Ground") || m_Anim.GetBool("Climb") || m_Anim.GetBool("Swing"))
                swingDisabled = SwingDisableStatus.Disabled;

        if (swingDisabled == SwingDisableStatus.Disabled) // if swing disabled and have been grounded
        { // keep track of whether swing is enabled
            if (swingDisableCounter >= swingDisableTime)
            {
                swingDisabled = SwingDisableStatus.enabled;
                swingDisableCounter = 0;
                disabledVineRoot = null;
            }
            swingDisableCounter += Time.deltaTime;
        }
    }

    public void Move(float moveHor, float moveVer, bool jump)
    {
        if (m_Anim.GetBool("Swing"))
        { // if currently swinging, give control to the swing function
            Swing(moveHor, moveVer, jump);
            return;
        }
        else if (m_Anim.GetBool("Climb"))
        { // if currently climbing, give control to the climb function
            Climb(moveHor, moveVer, jump);
            return;
        }
        else if (chooseVineLink(moveVer, jump))
        { // if there's a vine available that is not disabled, and the player presses up or space (or autoattach is enabled)
            StartSwinging(chooseVineLink(moveVer, jump));
        }

        else if (availableForClimb && moveVer > 0)
        { // if there's an object you can climb and user presses up
            if (!(climbDisabled && disabledClimbObject == availableForClimb)) // make sure climb is enabled for this object
                StartClimbing(availableForClimb);
        }


        //if not climbing or swinging - only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // The Speed animator parameter is set to the absolute value of the horizontal input.
            m_Anim.SetFloat("Speed", Mathf.Abs(moveHor));

            // Move the character
            m_Rigidbody2D.velocity = new Vector2(moveHor * m_MaxSpeed, m_Rigidbody2D.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (moveHor > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (moveHor < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump && m_Anim.GetBool("Ground"))
        {
            Jump();
        }
    }


    private void Climb(float moveHor, float moveVer, bool jump)
    {
        Debug.Log("Climbing");

        if (jump)
        {   // detach from tree, jump
            StopClimbing();
            Jump();
        }

        else if (moveVer < 0 && m_Grounded)
        { // Stop climbing if you're at the floor and press down
            StopClimbing();
        }

        else
        {
            if (!treeScript.allowHorizontal)
            {
                moveHor = 0;
            }
            else if (
               (moveHor < 0 && Helper.GetEdge(m_bodyCollider, "LEFT") <= treeScript.getBound("LEFT"))
            || (moveHor > 0 && Helper.GetEdge(m_bodyCollider, "RIGHT") >= treeScript.getBound("RIGHT")))
            { // player is trying to leave the climbable area, kill his horizontal movement
                Debug.Log("killing horizontal movement. moveHor: " + moveHor);
                moveHor = 0;
            }

            if (treeScript.restrictVertical)
            {
                if (moveVer > 0 && m_CeilingCheck.position.y >= treeScript.getBound("TOP")) // if head has reached top of climbing area, and player presses up
                    moveVer = 0; // kill vertical movement
            }

            float speedMultiplier = treeScript.climbEase * m_climbSpeed; // take player climb speed as well as tree climb ease into consideration
            m_Rigidbody2D.velocity = new Vector2(moveHor, moveVer) * speedMultiplier;
        }
    }
    public void StartClimbing(GameObject obj)
    {
        climbingOnThis = obj;
        m_Rigidbody2D.gravityScale = 0;
        m_Grounded = false;
        m_Anim.SetBool("Climb", true);
        treeScript = climbingOnThis.GetComponent<ClimbableTree>();
    }
    public void StopClimbing()
    {
        //Debug.Log("STOP CLIMBING METHOD CALLED");
        m_Rigidbody2D.gravityScale = m_gravityScaleDefault;
        m_Anim.SetBool("Climb", false);
        tempDisableClimb(climbingOnThis);
        climbingOnThis = null;
        treeScript = null;

    }
    private void tempDisableClimb(GameObject wasClimbingOnThis)
    {
        //Debug.Log("tempDisableClimb() called");
        climbDisabled = true;
        climbDisableCounter = 0;
        disabledClimbObject = wasClimbingOnThis;
    }


    private void Swing(float moveHor, float moveVer, bool jump)
    {
        if (jump)
        {
            StopSwinging();
        }
        else
        {
            // set rotation to vine node connected to and then add offset
            transform.rotation = swingingOnThis.transform.rotation;
            transform.Rotate(swingRotationOffset);

            float speedMultiplier = m_swingSpeed * vineScript.swingEase;
            Vector2 userInput = new Vector2(moveHor, 0);

            //Debug.Log(string.Format("Adding force {0} to object {1}",userInput* speedMultiplier, swingingOnThis.name ));
            swingingOnThis.GetComponent<Rigidbody2D>().AddForce(userInput * speedMultiplier);

            //if (allowSlideDownVine && moveVer < 0 && !swingingOnThis.GetComponent<VineLink>().isLastLink)      // if shimmying down a vine is allowed and player presses down, and not on the last link
            //{
            //    swingJoint.connectedAnchor -= ((Vector2) swingingOnThis.transform.up) * m_climbSpeed * 0.01f; //lower anchor of joint connecting player and vine. use climbspeed to determine how fast this 
            //}


            // If the input is moving the player right and the player is facing left...
            if (moveHor > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (moveHor < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
    }
    private void StartSwinging(GameObject chosenVineLink)
    {
        Debug.Log("StartSwinging() called");
        swingingOnThis = chosenVineLink;
        
        m_Rigidbody2D.velocity = Vector2.zero;                                  // kill velocity

        // if facing right, subtract the x offset as opposed to adding it, so player is in front of vine (assumes offset has positive X)
        Vector2 posOffset = (m_FacingRight) ? new Vector2(-swingPositionOffset.x, swingPositionOffset.y) : swingPositionOffset;
        transform.position = chosenVineLink.transform.position + (Vector3)posOffset;

        transform.rotation = chosenVineLink.transform.rotation;                 // set player rotation to vine
        transform.Rotate(swingRotationOffset);                                  // rotate more by offset

        // add a joint to fix distance between the vine and the player
        swingJoint = gameObject.AddComponent<HingeJoint2D>();                   // attach a joint to maintain distance from vine
        swingJoint.connectedBody = swingingOnThis.GetComponent<Rigidbody2D>();
        //swingJoint.dampingRatio = 1;                                            // dampen the shit out of that movement
        swingJoint.autoConfigureConnectedAnchor = false;                        // USED TO SLIDE DOWN VINE, CURRENTLY NOT IN USE
        m_Grounded = false;
        m_Anim.SetBool("Swing", true);
        vineScript = swingingOnThis.GetComponent<VineLink>();
    }
    public void StopSwinging()
    {
        Debug.Log("StopSwinging() called");
        Destroy(swingJoint);
        m_Anim.SetBool("Swing", false);

        transform.rotation = Quaternion.identity; // reset rotation

        tempDisableSwing(swingingOnThis);
        swingingOnThis = null;
        vineScript = null;
    }
    private void tempDisableSwing(GameObject wasSwingingOnThis)
    {
        //Debug.Log("tempDisableSwing() called");
        swingDisabled = SwingDisableStatus.waitingForGroud; // wait until the player touches the ground, then start counting the disable time
        disabledVineRoot = vineScript.vineRoot;
        swingDisableTime = disabledVineRoot.GetComponent<Vine>().swingDisableTime;
        swingDisableCounter = 0;

    }
    private GameObject chooseVineLink(float moveVer, bool jump)
    {
        if (availableForSwing.Count == 0) // if there are no vines available, return false
            return null;

        var relevantLinks = new List<GameObject>();

        if (!disabledVineRoot) // if no root is disabled, test all links in availableForSwing
            relevantLinks = availableForSwing;
        else
            foreach (GameObject link in availableForSwing) // add only links whose vine is enabled
                if (link.GetComponent<VineLink>().vineRoot != disabledVineRoot)
                    relevantLinks.Add(link);

        if (relevantLinks.Count == 0)
            return null;

        if (moveVer > 0 || jump) // if the player pressed up or space, simply return the first link in the list
                return relevantLinks[0];

        // if there are objects available for swing and the player didn't press, check if auto attach is enabled
        if (vineAutoAttach)
        { // check if links' root have auto attach override enabled, return the first applicable link else null
            foreach (GameObject link in relevantLinks)
            {
                Vine root = link.GetComponent<VineLink>().vineRoot.GetComponent<Vine>();
                if (!root.overrideAutoAttach || root.autoAttach == true)
                    return link;
            }
            return null; // no applicable link found, return null
        }
        else
        { // if auto attach disabled, check if one of the vines has overriden this
            foreach (GameObject link in relevantLinks)
            {
                Vine root = link.GetComponent<VineLink>().vineRoot.GetComponent<Vine>();
                if (root.overrideAutoAttach && root.autoAttach == true)
                    return link;
            }
            return null; // no applicable link found, return null
        }
    }
    private GameObject getClosestVine()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector3.right, 20, LayerMask.GetMask("Vines"));
        return hitInfo.transform.gameObject;
    }

    void Jump()
    {
        // Add a vertical force to the player.
        m_Grounded = false;
        m_Anim.SetBool("Ground", false);
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    //public pethods
    public Transform getGroundCheck()
    {
        return m_GroundCheck;
    }
    public Transform getCeilingCheck()
    {
        return m_CeilingCheck;
    }

	public void playClip(int index)
	{
		sfxCtr.playFX (index);
	}
}

