using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

	public static GameController gcInstance;
	private Vector2 checkpoint;
	private bool isRespawning;					//to check if the player is in the process of respawing

	public int totalScenes;
	public AnimationClip fadeColorAnimationClip;

	void Awake()
	{
		if (gcInstance == null) {				//check if the gameobject already exists
			gcInstance = this;					//if not get reference and don't destroy it on load
			DontDestroyOnLoad (gcInstance);
		} 
		else
			Destroy (this.gameObject);			//if gameobject is already present destroy duplicate gameobject
	}
//
//	void OnLevelWasLoaded()
//	{
//		if (isRespawning) {
//			GameObject scout = Helper.GetPlayer();					//get referece of player in current scene
//			scout.transform.position = new Vector3 (checkpoint.x, checkpoint.y, 0.0f);		//set position to checkpoint
//			isRespawning = false;
//		}
//	}

	public void setCheckpoint (float x, float y)
	{
		checkpoint.x = x;
		checkpoint.y = y;
	}

	public Vector2 getCheckpoint ()
	{
		return checkpoint;
	}

	public void respawn()
	{
//		isRespawning = true;
//		openScene (SceneManager.GetActiveScene().buildIndex);
		GameObject scout = Helper.GetPlayer();					                        // get referece of player in current scene
		scout.transform.position = new Vector3 (checkpoint.x, checkpoint.y, 0.0f);		// set position to checkpoint
        scout.GetComponent<Rigidbody2D>().velocity = Vector2.zero;                      // kill velocity

	}

	private void openScene(int sceneNum)
	{
		if (sceneNum >= totalScenes)
			sceneNum = 0;
		SceneManager.LoadScene (sceneNum);

	}

	private void loadDelayed()
	{
		
		openScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

	public void changeScene()
	{
		this.GetComponentInChildren<Animator> ().SetTrigger ("fade");
		Invoke("loadDelayed",fadeColorAnimationClip.length * 0.5f);
	}


}
