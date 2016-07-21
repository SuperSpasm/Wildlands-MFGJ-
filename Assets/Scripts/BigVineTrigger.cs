using UnityEngine;
using System.Collections;

public class BigVineTrigger : MonoBehaviour {

    public GameObject player;
    public Joint2D releaseJoint;

    private ScoutController scoutControl;
    private ScoutUserControl scoutUserControl;
    private bool triggered;
    

    void Awake()
    {
        scoutControl= player.GetComponent<ScoutController>();
        scoutUserControl = player.GetComponent<ScoutUserControl>();
        triggered = false;
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //Debug.Log(string.Format("trigger entered. gameObject: {0}, collider tag: {1}, object tag: {2}", otherCollider.gameObject.name, otherCollider.tag, otherCollider.gameObject.tag));
        if(otherCollider.gameObject.tag == "Player")
        {
            if (!scoutUserControl.disableMovement)
            {
                Debug.Log("disable movement false");
                scoutUserControl.disableMovement= true;
                triggered = true;
            }
        }
    }
    void Update()
    {
        //Debug.Log(string.Format("triggered: {0}, swingingOn: {1}", triggered.ToString(), scoutControl.swingingOnThis ? scoutControl.swingingOnThis.ToString() : "null"));
        if (triggered && scoutControl.swingingOnThis)
        { // if triggered and player swinging (on big vine)
            Debug.Log("Zhu lee, Do the thing!");
            releaseJoint.enabled = false;
        }
    }
}
