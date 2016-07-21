using UnityEngine;
using System.Collections;

public class TEST : MonoBehaviour {
    public enum Mode { Start, Update, FixedUpdate, ContextMenuOnly }
    public Mode whenToDoStuff;

    public enum StuffThisCanDo {outPutPosition, outPutColliderPos, findClosestVine, outPutTopBorder, outPutBottomBorder, outPutLeftBorder, outPutRightBorder }
    public StuffThisCanDo whatToDo;

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

            case StuffThisCanDo.outPutTopBorder:
                Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "TOP"));
                break;
            case StuffThisCanDo.outPutBottomBorder:
                Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "BOTTOM"));
                break;
            case StuffThisCanDo.outPutRightBorder:
                Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "RIGHT"));
                break;
            case StuffThisCanDo.outPutLeftBorder:
                Debug.Log(Helper.GetEdge(GetComponent<BoxCollider2D>(), "LEFT"));
                break;
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
