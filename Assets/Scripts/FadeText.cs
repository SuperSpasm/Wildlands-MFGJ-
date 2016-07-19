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

	public void changeText()	//called at the end of fade out animation clip
	{
		if (iterator >= textstrings.Length) {
							//this is temporary
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
		textGroup.alpha = 0.0f;
		fade = true;
		iterator = 0;
		changeText ();
		playAnim ();
	}

	void Update () {
        if (Input.GetButtonUp("Submit"))
            Debug.Log("submit let go");
        if(Input.GetButtonUp ("Submit") || iterator == textstrings.Length){
            fadeTextAnim.SetTrigger("finish");
        }
		else if (Input.GetButtonUp ("Jump") && !fade) {
			playAnim ();
		}
	}

    public void loadNextLevel() {
        SceneManager.LoadScene(2);
    }

}
