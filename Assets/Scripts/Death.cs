using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Death : MonoBehaviour {

	private GameObject scout;
	private Vector2 checkpoint;
	private GameObject gameController;
	private GameController gameManager;


	void Awake()
	{
		scout = GameObject.FindGameObjectWithTag ("Player");
//		gameController = GameObject.FindGameObjectWithTag ("GameController");
//		gameManager = gameController.GetComponent<GameController> ();
	}

	void OnTriggerEnter2D ( Collider2D otherCollider)
	{
		if (otherCollider.gameObject.GetHashCode() == scout.GetHashCode()) {
//			checkpoint = gameManager.getCheckpoint ();
//			gameManager.respawn ();
//			checkpoint = GameController.gcInstance.getCheckpoint();
			GameController.gcInstance.respawn ();
//			scout = GameObject.FindGameObjectWithTag ("Player");
//			scout.transform.Translate (checkpoint.x, checkpoint.y, 0.0f);

		}
	}
}
