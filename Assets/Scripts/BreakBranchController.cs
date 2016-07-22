using UnityEngine;
using System.Collections;

public class BreakBranchController : MonoBehaviour {

	private BreakBranch breakScript;
	private Vector3 initPos;
	private Vector3 initRot;

	void Start ()
	{
		initPos = this.gameObject.transform.position;
		initRot = this.gameObject.transform.eulerAngles;
	}

	void Awake ()
	{
		breakScript = GetComponent<BreakBranch> ();
	}
	void OnTriggerEnter2D (Collider2D otherCollider)
	{

        if (otherCollider.tag == "player_tag")
        {
            breakScript.initPos = initPos;
			breakScript.initRot = initRot;
			breakScript.enabled = true;
			this.enabled = false;
			return;
		}
	}

}
