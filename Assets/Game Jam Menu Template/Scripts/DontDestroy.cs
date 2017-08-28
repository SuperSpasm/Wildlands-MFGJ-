using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DontDestroy : MonoBehaviour {
    [Tooltip("will only destroy this object if the particular scene has been loaded. -1 -> never destroy. \nthis will NOT be destroyed in the same scene it was created in")]
    public int sceneToDestroy= -1;
    [Tooltip("a hack to fix transferring audio sources with 3d rolloff to the next scene.")]
    public bool resetPosOnLoad = false;
    public Vector2 newPos;
    private bool stillInOriginScene = true;                    // used to determine whether a scene has passed since this object was created

	void Start()
	{
        if (stillInOriginScene)
            stillInOriginScene = false;
        transform.SetParent(null);                        // unparent so that DontDestroyOnLoad will work
		DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (stillInOriginScene)
        {
            stillInOriginScene = false;
            return;
        }
        if (resetPosOnLoad)
            transform.position = newPos;
        if (scene.buildIndex == sceneToDestroy)
        {
            Debug.Log("destroying " + gameObject.name);
            Destroy(gameObject);
        }
    }

	

}
