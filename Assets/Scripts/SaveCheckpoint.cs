using UnityEngine;
using System.Collections;

public class SaveCheckpoint : MonoBehaviour {

	private GameObject scout;

	void Awake () {
		scout = GameObject.FindGameObjectWithTag ("Player");
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		if (otherCollider.gameObject == scout) {
			GameManager.Instance.setCheckpoint(scout.transform.position.x, scout.transform.position.y);
	
		}
	}
}
