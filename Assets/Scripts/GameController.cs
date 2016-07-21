using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {


	private Vector2 checkpoint;

	public int totalScenes;
	public AnimationClip fadeColorAnimationClip;

	public void setCheckpoint (float x, float y)
	{
		checkpoint.x = x;
		checkpoint.y = y;
	}

	public Vector2 getCheckpoint ()
	{
		return checkpoint;
	}

	public Vector2 respawn()
	{
		openScene (SceneManager.GetActiveScene().buildIndex);
		return getCheckpoint ();
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
