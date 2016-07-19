using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager{

	private static GameManager instance = new GameManager ();

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
}
