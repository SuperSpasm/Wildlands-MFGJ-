using UnityEngine;
using System.Collections;

public class SaveCheckpoint : MonoBehaviour {

	private GameObject scout;
	private GameObject gameController;

	void Awake () {
		scout = GameObject.FindGameObjectWithTag ("Player");
		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		if (otherCollider.gameObject == scout) {
			gameController.GetComponent<GameController> ().setCheckpoint (scout.transform.position.x, scout.transform.position.y);
	
		}
	}
}
