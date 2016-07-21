using UnityEngine;
using System.Collections.Generic;

public class VineEditor : MonoBehaviour {

    [Space]
    public GameObject lastLink;
    [Header("Renaming")]
    public string renameTo = "vine node";
    private List<GameObject> m_listedLinks;

    

    [Header("Adding Children")]
    public GameObject addToChildren;
    public enum PositionMode { setToOrigin, keepLocalPosition, automaticPosition }
    public enum ScaleMode    { keepLocalScale   , automaticScale }
    public enum RotationMode { keepLocalRotation, automaticRotation }
    
    public PositionMode positionMode;
    public ScaleMode    scaleMode;
    public RotationMode rotationMode;
    
    
    [ContextMenu("Rename VineLinks")]
    void renameLinks()
    {
        getLinks(); // gets the links from last to first
        int n = m_listedLinks.Count;
        foreach(GameObject link in m_listedLinks)
        {
            Debug.Log(string.Format("renaming {0} to: {1} ({2:D3})", link.name, renameTo, n));
            link.name = string.Format("{0} ({1:D3})", renameTo, n);
            link.transform.SetSiblingIndex(n - 1);
            n--;

        }
    }

    [ContextMenu("Add to Node's children")]
    void addObjectToChildren()
    {
        GameObject newInstance;
        getLinks();
        foreach(GameObject link in m_listedLinks)
        {
            newInstance = Instantiate(addToChildren);
            Debug.Log("instance name: " + newInstance.name + " endsWith? " + newInstance.name.EndsWith("(Clone)"));
            // if name ends with "(Clone)"- remove this
            string name = newInstance.name;
            if (name.EndsWith("(Clone)"))
            {
                newInstance.name = name.Substring(0, name.Length - 7);
            }
            Debug.Log("scale before parenting (global): " + newInstance.transform.lossyScale);
            newInstance.transform.SetParent(link.transform, true); // set newInstance transform with its global transform and scale
            Debug.Log("scale after parenting (global): " + newInstance.transform.lossyScale);

            if (positionMode == PositionMode.keepLocalPosition)
                newInstance.transform.localPosition = addToChildren.transform.localPosition;

            else if (positionMode == PositionMode.setToOrigin)
                newInstance.transform.localPosition = Vector3.zero;

            if (scaleMode == ScaleMode.keepLocalScale)
                newInstance.transform.localScale = addToChildren.transform.localScale;

            if (rotationMode == RotationMode.keepLocalRotation)
                newInstance.transform.localRotation = addToChildren.transform.localRotation;
        }
    }

    [ContextMenu("Remove Node's children")]
    void removeChildren()
    {
        getLinks();
        foreach (GameObject link in m_listedLinks)
        {
            foreach (Transform child in link.transform)
                DestroyImmediate(child.gameObject);
        }
    }


    void getLinks()
    {
        m_listedLinks = new List<GameObject>(); // start with clean list

        GameObject currLink;
        Rigidbody2D nextBody = lastLink.GetComponent<Rigidbody2D>(); // start with the rigidbody2d of the last link

        do
        {
            currLink = nextBody.gameObject; // get the gameobject & add to list

            if (m_listedLinks.Contains(currLink))
                throw new System.ArgumentException(string.Format("List already contains the gameObject {0}. something probably went wrong.", currLink.name));
            else
                m_listedLinks.Add(currLink);

            if (currLink.GetComponents<HingeJoint2D>().Length > 1)
                throw new System.Exception(string.Format("{0} HingeJoint2D's found on node {1}! only 1 (or 0 for last) may be present.", currLink.GetComponents<HingeJoint2D>().Length, currLink.name));

            nextBody = currLink.GetComponent<HingeJoint2D>().connectedBody; // get the conntected rigidbody2d (will be null on FIRST link)
        } while (nextBody);
    }

    void debugPrintList()
    {
        Debug.Log("Listing the list: ");
        foreach (GameObject link in m_listedLinks)
        {
            Debug.Log("\t" + link.name);
        }
    }


}
