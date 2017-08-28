using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    //OnLevelWasLoaded is called after a new scene has finished loading
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		//If there is no EventSystem (needed for UI interactivity) present
		if(!FindObjectOfType<EventSystem>())
		{
			//The following code instantiates a new object called EventSystem
			GameObject obj = new GameObject("EventSystem");

			//And adds the required components
			obj.AddComponent<EventSystem>();
			obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
		}
	}
}
