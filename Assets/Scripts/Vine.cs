using UnityEngine;
using System.Collections;

public class Vine : MonoBehaviour {
    [Tooltip("if checked, all vine chain links' swingEase var will be overriden with this")]
    public bool overrideEaseValues;
    public float swingEase;
    public float swingDisableTime;
    private VineLink[] chainLinks;

    void Awake()
    {

    }
	// Use this for initialization
	void Start () {

        if (overrideEaseValues)
            foreach (Transform child in transform)
            {
                if (child.GetComponent<VineLink>())
                    child.GetComponent<VineLink>().swingEase = swingEase;
            }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
