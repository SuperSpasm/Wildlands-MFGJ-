using UnityEngine;
using System.Collections;

public class TEST : MonoBehaviour {
    public enum Mode { Start, Update, FixedUpdate }
    public Mode whenToDoStuff;

    public enum StuffThisCanDo {outPutPosition, outPutLeftEdge, outPutRightEdge, outPutTopEdge, outPutBottomEdge }
    public StuffThisCanDo whatToDo;

    // Use this for initialization

    void doStuff()
    {
        switch (whatToDo)
        {
            case StuffThisCanDo.outPutPosition:
                Debug.Log(string.Format("{0} position: {1}", name, transform.position.ToString() ) );
                break;

            case StuffThisCanDo.outPutRightEdge:
                Debug.Log(string.Format("{0} [RIGHT] edge: {1}", name, Helper.GetEdge(GetComponent<Collider2D>(),"RIGHT")));
                break;
            case StuffThisCanDo.outPutLeftEdge:
                Debug.Log(string.Format("{0} [LEFT] edge: {1}", name, Helper.GetEdge(GetComponent<Collider2D>(), "LEFT")));
                break;
            case StuffThisCanDo.outPutTopEdge:
                Debug.Log(string.Format("{0} [TOP] edge: {1}", name, Helper.GetEdge(GetComponent<Collider2D>(), "TOP")));
                break;
            case StuffThisCanDo.outPutBottomEdge:
                Debug.Log(string.Format("{0} [BOTTOM] edge: {1}", name, Helper.GetEdge(GetComponent<Collider2D>(), "BOTTOM")));
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
