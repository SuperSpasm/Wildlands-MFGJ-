using UnityEngine;
using System.Collections;

public class BreakBranch : MonoBehaviour {

	public float timeToFall;
	private Rigidbody2D rb;
	private float elapsedTime;

	void Awake () {
		elapsedTime = 0.0f;
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime >= timeToFall) {
			rb.gravityScale = 1.0f;
			rb.isKinematic = false; 
			this.enabled = false;
			return;
		}

	}

}
