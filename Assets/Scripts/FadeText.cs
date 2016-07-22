using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeText : MonoBehaviour {

	private bool fade;
	private Animator fadeTextAnim;
	private CanvasGroup textGroup;
	private int iterator;
	public string [] textstrings;
	private Text text;
	private GameObject gameController;

	public void changeText()	//called at the end of fade out animation clip
	{
		if (iterator >= textstrings.Length) {
			text.text = "";
			gameController.GetComponent<GameController> ().changeScene ();				
			return;
		}
		text.text = "" + textstrings [iterator];
		++iterator;
	}

	public void playAnim()		//called at the end of fade out animation clip
	{
		fade = true;
		fadeTextAnim.SetBool ("fade", fade);
		fadeTextAnim.SetFloat ("Alpha", textGroup.alpha);
		fade = false;
	}

	void Awake()
	{
		fadeTextAnim = GetComponent<Animator> ();
		textGroup = GetComponent<CanvasGroup> ();
		text = GetComponent<Text> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController");
		textGroup.alpha = 0.0f;
		fade = true;
		iterator = 0;
		changeText ();
		playAnim ();
	}

	void Update () {

        // if user presses "enter" - cue the FadeToNextLevel animation
        // the animation then fades music, text and calls loadNextLevel() when its done
        if (Input.GetButtonUp("Submit") || iterator == textstrings.Length)
        {
            fadeTextAnim.SetTrigger("finish");
        }
        else
            if (Input.GetButtonUp ("Jump") && !fade) {
			playAnim ();
		}
	}

    public void loadNextLevel() {
		gameController.GetComponent<GameController> ().changeScene ();
    }

}
