using UnityEngine;
using System.Collections.Generic;

public class VineLinkRename : MonoBehaviour {
    public GameObject lastLink;
    public string renameTo = "Link";

    private List<GameObject> m_listedLinks;

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

            nextBody = currLink.GetComponent<HingeJoint2D>().connectedBody; // get the conntected rigidbody2d (will be null on FIRST link)
        } while (nextBody);

        debugPrintList();
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
