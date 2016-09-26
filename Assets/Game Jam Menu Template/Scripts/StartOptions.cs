using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class StartOptions : MonoBehaviour {



	public int sceneToStart = 1;										//Index number in build settings of scene to load if changeScenes is true
	public bool changeScenes;											//If true, load a new scene when Start is pressed, if false, fade out UI and continue in single scene

	[HideInInspector] public bool inMainMenu = true;					//If true, pause button disabled in main menu (Cancel in input manager, default escape key)
	[HideInInspector] public Animator animColorFade; 					//Reference to animator which will fade to and from black when starting game.
	[HideInInspector] public Animator animMenuAlpha;					//Reference to animator that will fade out alpha of MenuPanel canvas group
	public AnimationClip fadeColorAnimationClip;		                //Animation clip fading to color (black default) when changing scenes
	[HideInInspector] public AnimationClip fadeAlphaAnimationClip;		//Animation clip fading out UI elements alpha


    private MenuMusicController musicControl;                           //Reference to MenuMusicController script
	private float fastFadeIn = .01f;									//Very short fade time (10 milliseconds) to start playing music immediately without a click/glitch
	private ShowPanels showPanels;										//Reference to ShowPanels script on UI GameObject, to show and hide panels
	private GameObject gameController;
	
	void Awake()
	{
		//Get a reference to ShowPanels attached to UI object
		showPanels = GetComponent<ShowPanels> ();

        musicControl = GetComponent<MenuMusicController>();
		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ResetMainMenu();
        }   
    }

	public void StartButtonClicked()
	{

		//If changeScenes is true, start fading and change scenes halfway through animation when screen is blocked by FadeImage
		if (changeScenes) 
		{
			//Use invoke to delay calling of LoadDelayed by half the length of fadeColorAnimationClip
			//Invoke ("LoadDelayed", fadeColorAnimationClip.length * .5f);
			LoadDelayed();
			//Set the trigger of Animator animColorFade to start transition to the FadeToOpaque state.
			//animColorFade.SetTrigger ("fade");
		} 

		//If changeScenes is false, call StartGameInScene
		else 
		{
			//Call the StartGameInScene function to start game without loading a new scene.
			StartGameInScene();
		}

	}

	//Once the level has loaded, check if we want to call PlayLevelMusic
	void OnLevelWasLoaded(int sceneIndex)
	{
        if (sceneIndex == 0)                                            // if the game was won and the first scene (main menu) was reloaded
        {
            ResetMainMenu();
        }
    }


	public void LoadDelayed()
	{
		//Pause button now works if escape is pressed since we are no longer in Main menu.
		inMainMenu = false;

		//Hide the main menu UI element
		showPanels.HideMenu ();

		//Load the selected scene, by scene index number in build settings
		//SceneManager.LoadScene (sceneToStart);
		gameController.GetComponent<GameController>().changeScene();
	}

	public void HideDelayed()
	{
		//Hide the main menu UI element after fading out menu for start game in scene
		showPanels.HideMenu();
	}

	public void StartGameInScene()
	{
		//Pause button now works if escape is pressed since we are no longer in Main menu.
		inMainMenu = false;

		//Set trigger for animator to start animation fading out Menu UI
		animMenuAlpha.SetTrigger ("fade");
		Invoke("HideDelayed", fadeAlphaAnimationClip.length);
		Debug.Log ("Game started in same scene! Put your game starting stuff here.");
	}


	public void ResetMainMenu()
	{
        // reset the music and fade it in
        showPanels.ShowMenu();
    }
}
