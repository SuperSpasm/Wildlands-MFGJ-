using UnityEngine;
using System.Collections;
using System;

public class BigVineTrigger : MonoBehaviour {

    public GameObject scout;
    public Joint2D releaseJoint;
    public AreaEffector2D effector;
    public GameObject bigVine;
    public Collider2D colliderToEnable;
    public GameObject mainCamera;
    public float zoomSpeed = 1.0f;
    public enum WhatToDo {DisableUserControl, EnableUserConrol, EnableEffector, DisableEffector, DetachVine, DetachPlayer, EnableCollider, ZoomOut, ZoomIn, LoadNextLevel}
    public float timeTillNextSceneCall;
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
                    DoDetachPlayer();
                    break;
                case WhatToDo.EnableCollider:
                    colliderToEnable.enabled = true;
                    break;
                case WhatToDo.ZoomIn:
                    mainCamera.GetComponent<Animator>().SetFloat("zoomSpeed", zoomSpeed);
                    mainCamera.GetComponent<Animator>().SetBool("zoomedOut", false);
                    break;
                case WhatToDo.ZoomOut:
                    mainCamera.GetComponent<Animator>().SetFloat("zoomSpeed", zoomSpeed);
                    mainCamera.GetComponent<Animator>().SetBool("zoomedOut", true);
                    break;
                case WhatToDo.LoadNextLevel:
                    Invoke("callNextScene", timeTillNextSceneCall);
                    break;
            }
            triggered = true;
        }
        //else if (!triggered)
        //    Debug.Log(string.Format("{0} entered trigger, doesn't match expected {1}. mode: {2}", otherCollider.name, triggeringObject.name, whatToDo.ToString()));
    }

    public void callNextScene()
    {
        GameController.gcInstance.changeScene();
    }

    private void DoDetachPlayer()
    {
        scoutControl.StopSwinging();                                    // detach player from vine
        foreach (Transform vineNode in bigVine.transform)
        {
            if (vineNode.GetComponent<Collider2D>())
                vineNode.GetComponent<Collider2D>().enabled = false;    // disable node triggers so theres no way player will re-attach
        }
        scoutControl.availableForSwing = new System.Collections.Generic.List<GameObject>(); // reset available for swing list to avoid buggy behaviour
    }
}
