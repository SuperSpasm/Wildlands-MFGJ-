using UnityEngine;
using System.Collections.Generic;

public class VineLinkRename : MonoBehaviour {
    public GameObject lastLink;
    public string renameTo = "Link";

    private List<GameObject> m_listedLinks;

    [ContextMenu("Rename VineLinks")]
    void renameLinks()
    {
        getLinks();
        int n = m_listedLinks.Count;
        foreach(GameObject link in m_listedLinks)
        {
            Debug.Log(string.Format("renaming {0} to: {1} ({2:D3})", link.name, renameTo, n));
            link.name = string.Format("{0} ({1:D3})", renameTo, n);
            n--;
        }
    }
    void getLinks()
    {
        m_listedLinks = new List<GameObject>();
        Rigidbody2D nextBody = lastLink.GetComponent<Rigidbody2D>();
        do
        {
            HingeJoint2D[] joints = GetComponents<HingeJoint2D>();
            Debug.Log("Got joint[] items: ");
            foreach (HingeJoint2D joint in joints)
                Debug.Log("\t" + joint.ToString());

            HingeJoint2D relevantJoint = null;
            if (joints.Length == 0)
                relevantJoint = null;
            else if (joints.Length == 1)
            { // if theres only one attached
                relevantJoint = joints[0];
            }
            else
            { // there's more than one hingejoint attached
                foreach (HingeJoint2D joint in joints)
                {
                    if (joint.connectedBody)
                    { // find the joint with the connected rigidbody
                        if (relevantJoint) // throw an error if they both have rigidbodies attached
                            throw new System.Exception(string.Format("Can't rename: {0} has two joints with bodies attached!"));
                        else
                            relevantJoint = joint;
                    }
                }
            }

            // at this point relativeJoint should be either the joint that contains the next connected rigidbody, or null.

            if (!relevantJoint)
            {
                nextBody = null;
            }
            else
                nextBody = relevantJoint.connectedBody;

            // at this point nextBody shoud either be the next body, or null.


        } while (nextBody); // keep going until there are no more connected rigidbodies

        Debug.Log("DONE WITH GETTING LINKS. Count: " + m_listedLinks.Count.ToString());
    }
}
