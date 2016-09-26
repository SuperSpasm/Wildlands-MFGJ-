using UnityEngine;
using System.Collections;

public class VineError : System.Exception
{
    public VineError(string msg) : base(msg)
    {

    }
}

public class Vine : MonoBehaviour {
    [Tooltip("if checked, all vine chain links' swingEase var will be overriden with this")]
    public bool overrideEaseValues;
    public float swingEase;
    public float swingDisableTime;
    private Transform[] children;
    public bool overrideAutoAttach;
    public bool autoAttach;
    public bool overrideOffset;
    public Vector2 newOffset;

    void Awake()
    {
        // get references to child links

        children = new Transform[transform.childCount];
        int i = 0;
        foreach(Transform child in transform)
        {
                children[i] = child;
                i++;
        }

    }
	// Use this for initialization
	void Start ()
    {

        foreach (Transform child in children)
        {
            if (!child.GetComponent<VineLink>())
                continue;

            var link = child.GetComponent<VineLink>();
            if (overrideEaseValues) // if relevant, override the ease values of each link
                link.swingEase = swingEase;

        }
    }
}
