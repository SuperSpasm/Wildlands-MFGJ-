using UnityEngine;
using UnityEngine.Audio;

public class MenuMusicController : MonoBehaviour {
    // a script that provides some basic functionality for the background music.
    // namely - starting, stopping, and fading in and out.
    // this is meant to be used in animation events or by triggers.

    private GameObject mainMenu; // a reference to the main menu
    private AudioSource bgMusic;
    private AudioMixerSnapshot volUp;
    private AudioMixerSnapshot volDown;
    public bool startMusicOnTrigger;    // if true, music will fade in when player enters trigger
    public float transitionTime;        // the time over which to fade in the music in OnTriggerEnter


    void Awake()
    {
        // get references
        mainMenu = GameObject.FindGameObjectWithTag("MenuUI"); // get a reference to main menu by tag (should be set in 00MainMenu Scene)

        bgMusic  = mainMenu.GetComponent<AudioSource>();
        volUp    = mainMenu.GetComponent<PlayMusic>().volumeUp;
        volDown  = mainMenu.GetComponent<PlayMusic>().volumeDown;

    }

    void OnTriggerEnter2D (Collider2D otherCollider)
    {
        if (startMusicOnTrigger && !bgMusic.isPlaying)
        {
            bgMusic.Play();
            fadeInMusic(transitionTime);
        }
    }


    public void StopMusic()
    {
        bgMusic.Stop();
    }

    public void StartMusic()
    {
        bgMusic.Play();
    }

    public void fadeOutMusic(float transitionTime)
    {
        volDown.TransitionTo(transitionTime);
    }

    public void fadeInMusic(float transitionTime)
    {
        volUp.TransitionTo(transitionTime);
    }

}
