using UnityEngine;
using System.Collections;

public class BreakBranch : MonoBehaviour {

	public float timeToFall;
	public float timeToReset;
	private Rigidbody2D rb;
	private float elapsedTime;
	[HideInInspector]public Vector3 initPos;
	[HideInInspector]public Vector3 initRot;
	private BreakBranchController brkBrnchCtr;

	void Awake () {
		elapsedTime = 0.0f;
		rb = GetComponent<Rigidbody2D> ();
//		initPos = this.gameObject.transform.position;
		brkBrnchCtr = GetComponent<BreakBranchController> ();
	}

	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= timeToFall && elapsedTime < timeToReset) {
			rb.gravityScale = 1.0f;
			rb.isKinematic = false; 
//			this.enabled = false;
			return;
		} else if (elapsedTime >= timeToReset) {
			elapsedTime = 0.0f;
			rb.gravityScale = 0.0f;
			rb.isKinematic = true;
			this.gameObject.transform.eulerAngles = initRot;
			this.gameObject.transform.position = initPos;
			brkBrnchCtr.enabled = true;
			this.enabled = false;
			return;
		}

	}

}
