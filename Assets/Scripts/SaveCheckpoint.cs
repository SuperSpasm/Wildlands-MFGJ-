using UnityEngine;
using System.Collections;

public class SaveCheckpoint : MonoBehaviour {

    public Transform respawnLocation;
	private GameObject scout;
	private GameObject gameController;

	void Awake () {
		scout = GameObject.FindGameObjectWithTag ("Player");
//		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}

	void OnTriggerEnter2D (Collider2D otherCollider) {
		if (otherCollider.gameObject == scout) {
<<<<<<< HEAD
			gameController.GetComponent<GameController> ().setCheckpoint (respawnLocation.position.x, respawnLocation.position.y);
	
=======
//			gameController.GetComponent<GameController> ().setCheckpoint (scout.transform.position.x, scout.transform.position.y);
			GameController.gcInstance.setCheckpoint (this.transform.position.x, this.transform.position.y);

>>>>>>> 69bba0c6b84dc231bc2a082f3359d702c91a98b1
		}
	}
}
