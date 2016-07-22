using UnityEngine;
using System.Collections;

public class SaveCheckpoint : MonoBehaviour {

    public Transform respawnLocation;
	private GameObject scout;
	private GameObject gameController;

	void Awake () {
		scout = GameObject.FindGameObjectWithTag ("player_tag");
//		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		if (otherCollider.gameObject == scout) {

//			gameController.GetComponent<GameController> ().setCheckpoint (scout.transform.position.x, scout.transform.position.y);
			GameController.gcInstance.setCheckpoint (respawnLocation.position.x, respawnLocation.position.y);
		}
	}
}
