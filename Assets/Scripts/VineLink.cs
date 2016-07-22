using UnityEngine;
using System.Collections;

public class VineLink : MonoBehaviour
{
    public GameObject player;
    private ScoutController playerController;
    public float swingEase;

    public GameObject vineRoot;
    public bool isLastLink;

    private bool bodyIn = false; // has the body (box collider) of the player entered the trigger?
    private bool feetIn = false; // have the feet (circle collider) of the player entered the trigger?


    void Awake()
    {
        // set up references
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        playerController = player.GetComponent<ScoutController>();
        vineRoot = transform.parent.gameObject;
    }

    void Start()
    {
        // to make sure that the last link is set up correctly, throw an error if this is connected to a link that is set as 'last'
        Rigidbody2D body = GetComponent<HingeJoint2D>().connectedBody;
        if (body)
        {
            VineLink linkedToThis = body.GetComponent<VineLink>();
            if (linkedToThis.isLastLink)
                throw new VineError(string.Format("{0} is marked as last link, but {1} is connected to it!", linkedToThis.gameObject.name, gameObject.name));
        }
        
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // only react to the player
        if (otherCollider.gameObject == player)
        {
            //Debug.Log("entered swing trigger");

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

            SetSwingStatus();
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

            SetSwingStatus();
        }
    }

    private void SetSwingStatus()
    {
        // NOTE: this handles setting the availableForSwing references in the player controller
        //       the swingingOnThis and when to StartSwinging()/StopSwinging()
        //       is handled in the Move() method in the player controller.
        
        //Debug.Log("feet in? " + feetIn.ToString() + " body in? " + bodyIn.ToString() + "in list? " + playerController.availableForSwing.Contains(gameObject).ToString());
        if (bodyIn)
        { // if player is at least partially in the trigger, make sure this is available to swing on
            if(!playerController.availableForSwing.Contains(gameObject))
                playerController.availableForSwing.Add(gameObject);
        }
        if (!feetIn && !bodyIn)
        { // both the player's feet and body are out of the trigger area

            // if this is available for climb for the player
            if (playerController.availableForSwing.Contains(gameObject))
            {
                playerController.availableForSwing.Remove(gameObject);
            }
        }
    }

}
