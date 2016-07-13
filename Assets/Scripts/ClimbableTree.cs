﻿using UnityEngine;
using System.Collections;
using System;

public class ClimbableTree : MonoBehaviour {
    // A script for a tree that's climbable.
    // the basic script for climbable GameObjects
    [Tooltip("you can speed up or slow down the players climbing speed with this. 1 = default speed")]
    public GameObject player;

    public float climbEase;
    [Tooltip("if true, player will not be able to leave the climbing area from the sides")]
    public bool restrictToColliderArea; // NOTE: this functionality mostly implemented in ScoutController, using right/leftBounds

    private float leftBound;
    private float rightBound;

    private bool bodyIn = false; // has the body (box collider) of the player entered the trigger?
    private bool feetIn = false; // have the feet (circle collider) of the player entered the trigger?
    private ScoutController playerController;


    void Awake()
    {
        // set up references
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        playerController = player.GetComponent<ScoutController>();

        leftBound = transform.Find("Left Bound").position.x;
        rightBound = transform.Find("Right Bound").position.x;

    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // only react to the player
        if(otherCollider.gameObject == player)
        {
            Debug.Log("entered climb trigger");
            // see which part of the player entered the trigger 
            if (otherCollider.GetType() == typeof(BoxCollider2D))
            {
                bodyIn = true;
            }
            else if (otherCollider.GetType() == typeof(CircleCollider2D))
            {
                feetIn = true;
            }
            else
                throw new System.ArgumentException("The player should have ONLY ONE box collider and circle collider. recieved one that is neither.");

            SetClimbStatus();
        }
    }



    void OnTriggerExit2D(Collider2D otherCollider)
    {
        // only react to the player
        if (otherCollider.gameObject == player)
        {
            // see which part of the player exited the trigger 
            if (otherCollider.GetType() == typeof(BoxCollider2D))
            {
                bodyIn = false;
            }
            else if (otherCollider.GetType() == typeof(CircleCollider2D))
            {
                feetIn = false;
            }
            else
                throw new System.ArgumentException("The player should have ONLY ONE box collider and circle collider. recieved one that is neither.");

            SetClimbStatus();
        }
    }

    private void SetClimbStatus()
    {
        // NOTE: this handles setting the availableForClimb reference in the player controller
        //       and when to STOP climbing. STARTING to climb is handled in the Move() method in the player controller.

        if (feetIn || bodyIn)
        { // if player is at least partially in the trigger, make sure this is available to climb on
            playerController.availableForClimb = gameObject;
        }
        if (!feetIn && !bodyIn)
        { // both the player's feet and body are out of the trigger area
            
            // if this is available for climb for the player
            if (playerController.availableForClimb && playerController.availableForClimb == gameObject) 
                playerController.availableForClimb = null;
            
            // if player is climbing on this
            if (playerController.climbingOnThis && playerController.climbingOnThis == gameObject) 
                playerController.StopClimbing();
        }
    }

    public float getBound(string side)
    {
        if (side.ToUpper() == "LEFT")
            return leftBound;
        else if (side.ToUpper() == "RIGHT")
        {
            return rightBound;
        }
        else
            throw new ArgumentException("getBound(string) argument must be either \"LEFT\" or \"RIGHT\"!");
    }
}
