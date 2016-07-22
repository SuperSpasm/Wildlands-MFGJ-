using UnityEngine;
using System.Collections;

public class OneSidedPlatform : MonoBehaviour {
    public GameObject player;
    public GameObject PlatColliderObject; // the gameobject which holds the collider (likely a child of this)

    private Collider2D[] playerColliders;
    private Collider2D platCollider;

    private float platSurface; // the y coord of the platforms surface

    // true when the player is no longer on the trigger, but he's still neither above nor below the platform
    private bool playerGoingThroughPlat = false;

    void Awake()
    {
        // set up references
        platCollider = PlatColliderObject.GetComponent<Collider2D>();
        playerColliders = player.GetComponents<Collider2D>();
        platSurface = Helper.GetRealPos(platCollider).y;
        if (!player) // if player reference not set, search for it with tag
            player = GameObject.FindGameObjectWithTag("player_tag");
    }

    void Update()
    {
        if (playerGoingThroughPlat) // becomes true right after player leaves trigger
        {
            
            // wait until either players feet are above the platform, or his head is below it
            // then re-enable collisions.
            // NOTE: this may have unintended side effects if the player is allowed to go right
            //       or left and then return to the platform.
            float feetHeight = getPlayerFeetHeight();
            float headHeight = getPlayerHeadHeight();
            if (feetHeight >= platSurface || headHeight <= platSurface)
            {
                SetCollisionStatus(true); // re-enable collisions between player and platform
                playerGoingThroughPlat = false;
                //Debug.Log(string.Format("player above or below platform. resetting collisions. platHeight: {0}, feetHeight: {1}, headHeight: {2}", platSurface, feetHeight, headHeight));
            }
        }
        DebugLogIgnore();
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // ignore anything that isn't the player
        if (otherCollider.gameObject == player)
        {
            SetCollisionStatus(false); // disable collisions between platform and player
        }
    }

    void OnTriggerExit2D(Collider2D otherCollider)
    {
        // ignore anything that isn't the player
        if (otherCollider.gameObject == player)
        {
            playerGoingThroughPlat = true;
        }
    }


    private void SetCollisionStatus (bool willCollide)
    {
        // if [willCollide] is true - the player will collide with the platform.
        // if false - collisions will be ignored.
        foreach(Collider2D coll in playerColliders)
        {
            Physics2D.IgnoreCollision(coll, platCollider, !willCollide);
        }
    }

    bool GetCollisionsEnabled()
    {

        if (Physics2D.GetIgnoreCollision(playerColliders[0], platCollider)
        !=  Physics2D.GetIgnoreCollision(playerColliders[1], platCollider) )
            throw new System.Exception("Unknown error - only one of the player colliders is ignored by one-way platform.");

        else if (Physics2D.GetIgnoreCollision(playerColliders[0], platCollider)) // if the collisions are ignored
            return false;

        return true;
    }

    // debugging helper
    void DebugLogIgnore()
    {

        //Debug.Log(string.Format("player/platform collisions ignored? col1: {0}, col2: {1}"
        //                        , Physics2D.GetIgnoreCollision(playerColliders[0], platCollider)
        //                        , Physics2D.GetIgnoreCollision(playerColliders[1], platCollider)
        //                        )
        //         );
    }
    private float getPlayerFeetHeight()
    {
        return player.transform.Find("GroundCheck").position.y;
    }
    private float getPlayerHeadHeight()
    {
        return player.transform.Find("CeilingCheck").position.y;
    }
}
