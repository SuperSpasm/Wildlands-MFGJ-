using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoutController : MonoBehaviour
{

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

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
    // null if not climbing on, otherwise has a reference to the object currently climbing on
    [HideInInspector]
    public GameObject climbingOnThis;
    // will have a reference to a climbable object if you're in the vicinity of one. null otherwise.
    [HideInInspector]
    public GameObject availableForClimb;
    [SerializeField]
    private float m_climbSpeed;
    private float m_gravityScaleDefault;       // Since climbing disables gravity, keep track of original value
    private bool climbDisabled = false; // disable climbing for a short period after jumping

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
    // null if not swinging, otherwise has a reference to the object currently climbing on
    [HideInInspector]
    public GameObject swingingOnThis;
    // will have a reference to swingable objects if you're in the vicinity of some. empty otherwise.
    //[HideInInspector]
    public List<GameObject> availableForSwing;
    [SerializeField]
    private float m_swingSpeed;
    public bool allowVerticalSwing;

    FixedJoint2D swingJoint; // joint to be added to the player  attached to the vine while swinging
    private VineLink vineScript;

    [SerializeField]
    [Tooltip("after jumping from a swingable object swinging on that object will be disabled for this short time")]
    private float swingDisableTime;
    private bool swingDisabled;
    private float swingDisableCounter = 0;
    [SerializeField]
    private GameObject disabledVineRoot;

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
    }


    private void FixedUpdate()
    {
        GroundCheck();
        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

        ////debugging
        //if (availableForClimb)
        //    Debug.Log("Available for climb: " + availableForClimb);
        //else
        //    Debug.Log("Available for climb: " + "NONE");
        //if (climbingOnThis)
        //    Debug.Log("climbing on: " + climbingOnThis);
        //else
        //    Debug.Log("climbing on: " + "NONE");
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

        if (swingDisabled)
        { // keep track of whether swing is enabled
            if (swingDisableCounter >= swingDisableTime)
            {
                swingDisabled = false;
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
        else if (availableForSwing.Count > 0 && (moveVer > 0 || jump))
        { // if there's a vine available and the player presses up or space
            GameObject chosenLink = chooseVineLink(); //chosenLink will be null if theres no vine that's not in the disabled vineRoot!
            if (chosenLink) // if theres a link thats not on a disabled root
                StartSwinging(chosenLink);
        }

        else if (availableForClimb && moveVer > 0)
        { // if there's an object you can climb and user presses up
            if ( !(climbDisabled && disabledClimbObject == availableForClimb) ) // make sure climb is enabled for this object
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
            Debug.Log("climb->jump");
            StopClimbing();
            Jump();
        }

        else if (moveVer < 0 && m_Grounded)
        { // Stop climbing if you're at the floor and press down
            StopClimbing();
        }

        else
        {
            if (treeScript.restrictHorizontal)
            { // if restrictToColliderArea is true, make sure the player doesn't leave the tree's collider
                if (
                   (moveHor < 0 && Helper.GetEdge(m_bodyCollider, "LEFT") <= treeScript.getBound("LEFT"))
                || (moveHor > 0 && Helper.GetEdge(m_bodyCollider, "RIGHT") >= treeScript.getBound("RIGHT")))
                { // player is trying to leave the climbable area, kill his horizontal movement
                    Debug.Log("killing horizontal movement. moveHor: " + moveHor);
                    moveHor = 0;
                }
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
        Debug.Log("STOP CLIMBING METHOD CALLED");
        m_Rigidbody2D.gravityScale = m_gravityScaleDefault;
        m_Anim.SetBool("Climb", false);
        tempDisableClimb(climbingOnThis);
        climbingOnThis = null;
        treeScript = null;
        
    }
    private void tempDisableClimb(GameObject wasClimbingOnThis)
    {
        Debug.Log("tempDisableClimb() called");
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
            float speedMultiplier = m_swingSpeed * vineScript.swingEase;
            if (!allowVerticalSwing)
                moveVer = 0; //if vertcal swing is disallowed, disregaurd vertical input
            Vector2 userInput = new Vector2(moveHor, moveVer);
            //Debug.Log(string.Format("Adding force {0} to object {1}",userInput* speedMultiplier, swingingOnThis.name ));
            swingingOnThis.GetComponent<Rigidbody2D>().AddForce(userInput * speedMultiplier);


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
    private GameObject chooseVineLink()
    {
        if(!disabledVineRoot) // if no root is disabled, simply return first link in list
            return availableForSwing[0];
        //DEFAULT. will change this if it's problematic
        foreach (GameObject link in availableForSwing) // return the first link thats not in a disabled vineRoot
            if (link.GetComponent<VineLink>().vineRoot != disabledVineRoot)
                return link;

            return null;
    }
    private void StartSwinging(GameObject chosenVineLink)
    {
        Debug.Log("StartSwinging() called");
        swingingOnThis = chosenVineLink;
        transform.position = swingingOnThis.transform.position;
        // kill velocity
        m_Rigidbody2D.velocity = Vector2.zero;

        // add a joint to fix distance between the vine and the player
        swingJoint = gameObject.AddComponent<FixedJoint2D>(); // attach a joint to maintain distance from vine
        swingJoint.connectedBody = swingingOnThis.GetComponent<Rigidbody2D>();
        m_Grounded = false;
        m_Anim.SetBool("Swing", true);
        vineScript = swingingOnThis.GetComponent<VineLink>();
    }
    public void StopSwinging()
    {
        Destroy(swingJoint);
        m_Anim.SetBool("Swing", false);
        tempDisableSwing(swingingOnThis);
        swingingOnThis = null;
        vineScript = null;
    }
    private void tempDisableSwing(GameObject wasSwingingOnThis)
    {
        Debug.Log("tempDisableSwing() called");
        swingDisabled = true;
        disabledVineRoot = vineScript.vineRoot;
        swingDisableTime = disabledVineRoot.GetComponent<Vine>().swingDisableTime;
        swingDisableCounter = 0;

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

}

