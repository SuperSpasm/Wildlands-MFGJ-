using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager{

	private static GameManager instance = new GameManager ();
	private const int totalScenes = 4;
	private Vector2 checkpoint;

	private GameManager()
	{
		
	}

	public static GameManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}
	}

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

	public void openScene(int sceneNum)
	{
		if (sceneNum >= totalScenes)
			sceneNum = 0;
		SceneManager.LoadScene (sceneNum);
		
	}
}
