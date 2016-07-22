using UnityEngine;
using System.Collections;

public class BreakBranchController : MonoBehaviour {

	private BreakBranch breakScript;

	void Awake ()
	{
		breakScript = GetComponent<BreakBranch> ();
	}
	void OnTriggerEnter2D (Collider2D otherCollider)
	{
		if (otherCollider.tag == "player_tag") {
			breakScript.enabled = true;
			this.enabled = false;
			return;
		}
	}

}
