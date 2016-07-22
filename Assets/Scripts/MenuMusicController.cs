using UnityEngine;
using UnityEngine.Audio;

public class MenuMusicController : MonoBehaviour {
    // a script that provides some basic functionality for the background music.
    // namely - starting, stopping, and fading in and out.
    // this is meant to be used in animation events or by triggers.
    public static GameObject instance;

    private GameObject mainMenu; // a reference to the main menu
    private AudioSource bgMusic;

    public AudioMixer mainMixer;

    public bool cueMusicOnStart;
    public bool cueMusicOnTrigger;

    public bool fadeInVolOnStart;
    public bool fadeVolOnTrigger;    // if true, music will fade in when player enters trigger
    public float fadeTime;        // the time over which to fade in the music in OnTriggerEnter

    [Range(-80, 0)]
    [Tooltip("the volume in decibles to reach between 0 <-> (-80). this will be adjusted by a curve")]
    public float targetVolume;

    public enum FadeStatus { Idle, FadingIn, FadingOut }

    public FadeStatus fadeStatus;

    private float deltaVolume;  // the volume to add each second. ALWAYS POSITIVE (whether fading in or out)
    private float counter;

    private bool triggered = false;



    void Awake()
    {
        // get references
        mainMenu = GameObject.FindGameObjectWithTag("MenuUI"); // get a reference to main menu by tag (should be set in 00MainMenu Scene)
        bgMusic = mainMenu.GetComponent<AudioSource>();
        fadeStatus = FadeStatus.Idle;
    }

    void Start()
    {
        //Debug.Log(string.Format("deltavolume = target / time = {0} / {1} = {2}", targetVolume, fadeTime, deltaVolume));
        if (cueMusicOnStart && !bgMusic.isPlaying)
        {
            bgMusic.Play();
        }
        if (fadeInVolOnStart)
        {
            fadeInMusic();
        }
    }

    void Update()
    {


        switch (fadeStatus)
        {
            case FadeStatus.FadingIn:
                Debug.Log("Active! " + gameObject.name);
                if (instance && instance != gameObject) // if another instance of this script started during fade in / out - stop fading
                    fadeStatus = FadeStatus.Idle;
                else if (counter <= 0)
                {
                    SetVolume(targetVolume);
                    fadeStatus = FadeStatus.Idle;
                }
                else
                {
                    AddVolume(deltaVolume * Time.deltaTime);
                    counter -= Time.deltaTime;
                }
                break;

            case FadeStatus.FadingOut:
                if (instance && instance != gameObject) // if another instance of this script started during fade in / out - stop fading
                    fadeStatus = FadeStatus.Idle;
                else if (counter <= 0)
                {
                    SetVolume(targetVolume) ;
                    fadeStatus = FadeStatus.Idle;
                }
                {
                    AddVolume( -(deltaVolume * Time.deltaTime) ); // subtract delta volume from current
                    counter -= Time.deltaTime;
                }
                break;
        }
    }


    void OnTriggerEnter2D (Collider2D otherCollider)
    {

        if (fadeVolOnTrigger && otherCollider.tag == "Player" && !triggered)
        {
            triggered = true;
            if (!bgMusic.isPlaying)
                    bgMusic.Play();
                
                fadeInMusic();
            
        }
    }

    public void fadeInMusic()
    {
        float currVol;
        mainMixer.GetFloat("masterVol", out currVol);
        if (currVol > targetVolume) // if the volume is already higher than the target, do nothing
            return;

        instance = gameObject; // set this as the active instance;
        fadeStatus = FadeStatus.FadingIn;
        counter = fadeTime;

        //get deltaVol (current volume - target)
        mainMixer.GetFloat("masterVol", out currVol);
        deltaVolume = Mathf.Abs((currVol - targetVolume) / fadeTime);

        mainMixer.GetFloat("masterVol", out currVol);


    }
    public void fadeOutMusic()
    {
        instance = gameObject; // set this as the active instance;

        float currVol;
        mainMixer.GetFloat("masterVol", out currVol);

        //get deltaVol (current volume - target)
        mainMixer.GetFloat("masterVol", out currVol);
        deltaVolume = Mathf.Abs((currVol - targetVolume) / fadeTime);



        if (currVol < targetVolume)
            throw new System.ArgumentException(string.Format("Trying to fade out but current volume ({0}) is lower than target volume! ({1})", currVol, targetVolume));

        else
        {
            fadeStatus = FadeStatus.FadingOut;
            counter = fadeTime;
        }
    }

  


    private void SetVolume (float vol)
    {
        if (vol < -80 || vol > 0)
            throw new System.ArgumentOutOfRangeException("volume must be a float between 0 and -80!");
        else
            mainMixer.SetFloat("masterVol", vol);
    }
    private void AddVolume(float volToAdd)
    {
        float currVol;
        mainMixer.GetFloat("masterVol", out currVol);
        mainMixer.SetFloat("masterVol", currVol + volToAdd);
    }
}
