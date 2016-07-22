using UnityEngine;
using System.Collections;

public class BigVineTrigger : MonoBehaviour {

    public GameObject scout;
    public Joint2D releaseJoint;
    public AreaEffector2D effector;
    public GameObject bigVine;
    public Collider2D colliderToEnable;

    public enum WhatToDo {DisableUserControl, EnableUserConrol, EnableEffector, DisableEffector, DetachVine, DetachPlayer, EnableCollider }
    public WhatToDo whatToDo;

    private ScoutController scoutControl;
    private ScoutUserControl scoutUserControl;
    private bool triggered;
    

    void Awake()
    {
        scoutControl= scout.GetComponent<ScoutController>();
        scoutUserControl = scout.GetComponent<ScoutUserControl>();
        triggered = false;
    }


    //NOTE: these are used for the context menu functions in VineEditor
    public void DisableJoint()
    {
        releaseJoint.enabled = false;
    }
    public void EnableJoint()
    {
        releaseJoint.enabled = true;
    }


    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //Debug.Log(string.Format("trigger entered. gameObject: {0}, collider tag: {1}, object tag: {2}", otherCollider.gameObject.name, otherCollider.tag, otherCollider.gameObject.tag));
        if (otherCollider.gameObject.GetInstanceID() == scout.GetInstanceID() && !triggered)
        {
            //Debug.Log(string.Format("{0} entered trigger! will perform desired action. mode: {1}", otherCollider.name, whatToDo.ToString()));
            switch (whatToDo)
            {
                case WhatToDo.DisableUserControl:
                    scoutUserControl.disableMovement = true;
                    break;
                case WhatToDo.EnableUserConrol:
                    scoutUserControl.disableMovement = false;
                    break;
                case WhatToDo.EnableEffector:
                    effector.enabled = true;

                    break;
                case WhatToDo.DisableEffector:
                    effector.enabled = false;
                    break;
                case WhatToDo.DetachVine:
                    releaseJoint.enabled = false;
                    break;
                case WhatToDo.DetachPlayer:
                    scoutControl.StopSwinging();                                    // detach player from vine
                    foreach (Transform vineNode in bigVine.transform)
                    {
                        if (vineNode.GetComponent<Collider2D>())
                            vineNode.GetComponent<Collider2D>().enabled = false;    // disable node triggers so theres no way player will re-attach
                    }
                    scoutControl.availableForSwing = new System.Collections.Generic.List<GameObject>(); // reset available for swing list to avoid buggy behaviour
                    break;
                case WhatToDo.EnableCollider:
                    colliderToEnable.enabled = true;
                    break;
            }
            triggered = true;
        }
        //else if (!triggered)
        //    Debug.Log(string.Format("{0} entered trigger, doesn't match expected {1}. mode: {2}", otherCollider.name, triggeringObject.name, whatToDo.ToString()));
    }
}
