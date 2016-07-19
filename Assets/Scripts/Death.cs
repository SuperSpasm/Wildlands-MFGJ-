using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Death : MonoBehaviour {

	private GameObject scout;
	private Vector2 checkpoint;


	void Awake()
	{
		scout = GameObject.FindGameObjectWithTag ("Player");
	}

	void OnTriggerEnter2D ( Collider2D otherCollider)
	{
		if (otherCollider.gameObject.GetHashCode() == scout.GetHashCode()) {
			checkpoint = GameManager.Instance.getCheckpoint ();
			GameManager.Instance.respawn ();
			scout.transform.Translate (checkpoint.x, checkpoint.y, 0.0f);

		}
	}
}
