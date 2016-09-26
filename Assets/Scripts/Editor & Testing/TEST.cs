using UnityEngine;
using System.Collections;
using System;

public class TEST : MonoBehaviour {
    public enum Mode { Start, Update, FixedUpdate, ContextMenuOnly }
    public Mode whenToDoStuff;

    public enum StuffThisCanDo {outPutPosition, outPutColliderPos, findClosestVine, outPutBorder, DebugCollisions, getEdgeColliderPoints, GetHierarchy }
    public StuffThisCanDo whatToDo;

    [Header("Border options")]
    public Border border;
    public enum Border { Top, Bottom, Left, Right }
    [Header("Collision debug options")]
    public CollisionMode collisionMode;
    public enum CollisionMode { Both, Collisions, Triggers }
    public enum WhenToDetect { Stay, Enter, Exit, All }
    public WhenToDetect whenToDetect;

    [Header("Edge Testing")]
    public EdgeCollider2D edgeColl;
    public Transform edgeTester;
    

    // Use this for initialization

    [ContextMenu("Do Stuff")]
    void doStuff()
    {
        switch (whatToDo)
        {
            case StuffThisCanDo.outPutPosition:
                Debug.Log(string.Format("{0} position: {1}", name, transform.position.ToString() ) );
                break;

            case StuffThisCanDo.outPutColliderPos:
                Debug.Log(gameObject.name + " collider pos: " + Helper.GetRealPos(GetComponent<Collider2D>()) );
                break;

            case StuffThisCanDo.findClosestVine:
                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector3.right, 0.3f, LayerMask.GetMask("Vines"));
                Debug.Log("Closest vine: " + (hitInfo ? hitInfo.transform.gameObject.name : "null"));
                break;

            case StuffThisCanDo.getEdgeColliderPoints:
                int i = 0;
                Vector2 pointOffset = edgeColl.transform.position;
                Debug.Log("outputting line points: ");
                foreach (Vector2 point in edgeColl.points)
                {
                    Debug.Log(string.Format("\t{0}: {1}", i, point+pointOffset));
                    i++;
                }
                break;
            case StuffThisCanDo.GetHierarchy:
                Debug.Log("Heirarchy: "+Helper.GetHierarchy(gameObject));
                break;
                // end of main switch statement

            case StuffThisCanDo.outPutBorder:
                switch (border)
                {
                    case Border.Top:
                        Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "TOP"));
                        break;
                    case Border.Bottom:
                        Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "BOTTOM"));
                        break;
                    case Border.Right:
                        Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "RIGHT"));
                        break;
                    case Border.Left:
                        Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "LEFT"));
                        break;
                }
                break;
                // end of border case
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Collisions || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Enter || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(coll.gameObject.layer);
            Debug.Log(string.Format("collision enter. [this: {0} ({2}); colliding with: {1} ({3})]", name, coll.gameObject.name, thisLayer, collLayer));
        }
    }
    void OnCollisionStay2D(Collision2D coll)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Collisions || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Stay || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(coll.gameObject.layer);
            Debug.Log(string.Format("collision stay. [this: {0} ({2}); colliding with: {1} ({3})]", name, coll.gameObject.name, thisLayer, collLayer));
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Collisions || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Exit || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(coll.gameObject.layer);
            Debug.Log(string.Format("collision exit. [this: {0} ({2}); colliding with: {1} ({3})]", name, coll.gameObject.name, thisLayer, collLayer));
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Triggers || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Enter || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(otherCollider.gameObject.layer);
            //Debug.Log(string.Format("trigger enter. [this: {0} ({2}); colliding with: {1} ({3})]", name, otherCollider.gameObject.name, thisLayer, collLayer));
        }
    }
    void OnTriggerStay2D(Collider2D otherCollider)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Triggers || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Stay || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(otherCollider.gameObject.layer);
            Debug.Log(string.Format("trigger stay. [this: {0} ({2}); colliding with: {1} ({3})]", name, otherCollider.gameObject.name, thisLayer, collLayer));
        }
    }
    void OnTriggerExit2D(Collider2D otherCollider)
    {
        // if the action selected is to debug collisions, and relevant modes were selected
        if ((whatToDo == StuffThisCanDo.DebugCollisions)
        && (collisionMode == CollisionMode.Triggers || collisionMode == CollisionMode.Both)
        && (whenToDetect == WhenToDetect.Exit || whenToDetect == WhenToDetect.All))
        {
            string thisLayer = LayerMask.LayerToName(gameObject.layer);
            string collLayer = LayerMask.LayerToName(otherCollider.gameObject.layer);
            Debug.Log(string.Format("trigger exit. [this: {0} ({2}); colliding with: {1} ({3})]", name, otherCollider.gameObject.name, thisLayer, collLayer));
        }
    }



    void Start()
    {
        if (whenToDoStuff == Mode.Start)
            doStuff();
    }
	void Update () {
        if (whenToDoStuff == Mode.Update)
            doStuff();
    }
    void FixedUpdate()
    {
        if (whenToDoStuff == Mode.FixedUpdate)
            doStuff();
    }
}
