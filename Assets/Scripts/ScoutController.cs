using System;
using UnityEngine;

public class ScoutController : MonoBehaviour
{

    [SerializeField]
    private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
    [Range(0, 1)]
    [SerializeField]
    private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
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

    private float m_gravityScaleDefault;       // Since climbing disables gravity, keep track of original value

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
    private bool climbDisabled = false; // disable climbing for a short period after jumping

    [SerializeField]
    [Tooltip("after jumping from a climbable surface climbing will be disabled for this short time")]
    private float climbDisableTime = 0.2f;
    private float climbDisableCounter = 0;

    private ClimbableTree treeScript;
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

        m_gravityScaleDefault = m_Rigidbody2D.gravityScale;
    }


    private void FixedUpdate()
    {
        GroundCheck();
        // Set the vertical animation
        m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

        //debugging
        if (availableForClimb)
            Debug.Log("Available for climb: " + availableForClimb);
        else
            Debug.Log("Available for climb: " + "NONE");
        if (climbingOnThis)
            Debug.Log("climbing on: " + climbingOnThis);
        else
            Debug.Log("climbing on: " + "NONE");
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
                float colliderHeight = Helper.GetRealPos(colliders[i]).y;
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
            }
            climbDisableCounter += Time.deltaTime;
        }
    }

    public void Move(float moveHor, float moveVer, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch && m_Anim.GetBool("Crouch"))
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }
        // Set whether or not the character is crouching in the animator
        m_Anim.SetBool("Crouch", crouch);


        if (availableForClimb && moveVer > 0 && !climbDisabled)
        { // if there's an object you can climb and user presses up (and climb is enabled)
            StartClimbing(availableForClimb);
        }

        if (m_Anim.GetBool("Climb"))
        { // if currently climbing
            Climb(moveHor, moveVer, crouch, jump);
        }

        //if not climbing - only control the player if grounded or airControl is turned on
        else if (m_Grounded || m_AirControl)
        {
            // Reduce the speed if crouching by the crouchSpeed multiplier
            moveHor = (crouch ? moveHor * m_CrouchSpeed : moveHor);

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


    private void Climb(float moveHor, float moveVer, bool crouch, bool jump)
    {
        Debug.Log("Climbing");

        if (jump)
        {   // detach from tree, jump, then return control to Move()
            Debug.Log("climb->jump");
            StopClimbing();
            tempDisableClimb();
            Jump();
            return; 
        }

        

        float speedMultiplier = treeScript.climbEase * m_climbSpeed;


        m_Rigidbody2D.velocity = new Vector2(moveHor , moveVer ) * speedMultiplier;


        if (moveVer < 0 && m_Grounded)
        { // Stop climbing if you're at the floor and press down
            StopClimbing();
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
        climbingOnThis = null;
        treeScript = null;
    }

    private void tempDisableClimb()
    {
        Debug.Log("tempDisableClimb() called");
        climbDisabled = true;
        climbDisableCounter = 0;
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

