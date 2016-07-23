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
    private VineLink[] chainLinks;
    public bool overrideAutoAttach;
    public bool autoAttach;

    void Awake()
    {
        // get references to child links

        chainLinks = new VineLink[transform.childCount];
        int i = 0;
        foreach(Transform child in transform)
        {
            if (child.GetComponent<VineLink>())
            {
                chainLinks[i] = child.GetComponent<VineLink>();
                i++;
            }
        }

    }
	// Use this for initialization
	void Start () {

        short lastLinkCount = 0;

        foreach (VineLink link in chainLinks)
        {
            if (link.isLastLink)
            {         // count how many links have "isLastLink" marked
                lastLinkCount++;
            }

            if(overrideEaseValues) // if relevant, override the ease values of each link
                link.swingEase = swingEase;
        }

        if (lastLinkCount == 0)
            //throw new VineError(string.Format("No last link set on {0}!", gameObject.name));

        if (lastLinkCount > 1)
        {
            //throw new VineError(string.Format("There must only be one last link on {0}!", gameObject.name));
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
