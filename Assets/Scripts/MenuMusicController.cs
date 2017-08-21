using System;
using UnityEngine;
using UnityEngine.Audio;

public class MenuMusicController : MonoBehaviour {
    // a script that provides some basic functionality for the background music.
    // namely - starting, stopping, and fading in and out.
    // this is meant to be used in animation events or by triggers.
    public static MenuMusicController BGMinstance;
    public static MenuMusicController SFXinstance;

    private GameObject mainMenu; // a reference to the main menu
    private AudioSource bgMusic;


    public AudioMixer mainMixer;
    public enum Action { CueMusic, FadeInVol, FadeOutVol, FadeToVol, ChangeClip }
    public Action action = Action.FadeInVol;            // default to fade in trigger

    public enum ActivationTime { OnStart, OnTrigger, MethodsOnly, OnStartOrLevelLoad }
    public ActivationTime activationTime = ActivationTime.OnTrigger;
    public int LevelToActivateIn;

    public enum Mixer { musicTopVol, sfxTopVol }
    public Mixer mixer;
    public AudioClip substituteClip;

    public float fadeTime;                              // the time over which to fade in the music in OnTriggerEnter

    [Range(-80, 0)]
    [Tooltip("the volume in decibles to reach between 0 <-> (-80). this will be adjusted by a curve")]
    public float targetVolume;

    public enum FadeStatus { Idle, FadingIn, FadingOut }
    private FadeStatus fadeStatus;

    private string mixerGroup;
    private float deltaVolume;                          // the volume to add each second. ALWAYS POSITIVE (whether fading in or out)
    private float counter;

    private bool triggered = false;



    void Awake()
    {
        // get references
        mainMenu = EnsureUnique.uniqueGameObjects["MenuUI"]; // get a reference to main menu by tag (should be set in 00MainMenu Scene)
        bgMusic = mainMenu.GetComponent<AudioSource>();
        fadeStatus = FadeStatus.Idle;

    }

    void doStuff()
    {
        switch (action)
        {
            case Action.FadeInVol:
                FadeInVol();
                break;
            case Action.FadeOutVol:
                FadeOutVol();
                break;
            case Action.FadeToVol:
                FadeToVol();
                break;
            case Action.CueMusic:
                CueMusic();
                break;
            case Action.ChangeClip:
                ChangeClip();
                break;
        }
    }

    void Start()
    {
        if (activationTime == ActivationTime.OnStart || activationTime == ActivationTime.OnStartOrLevelLoad)
        {   // only do stuff if set to operate at start
            //Debug.Log(string.Format("deltavolume = target / time = {0} / {1} = {2}", targetVolume, fadeTime, deltaVolume));

            doStuff();
        }
    }

    void OnLevelWasLoaded(int levelIndex)
    {
        //Debug.Log(string.Format("OnLevelWasLoaded() called. current scene = {0}, ActivationScene = {1}", levelIndex, LevelToActivateIn));
        if (activationTime == ActivationTime.OnStartOrLevelLoad && levelIndex == LevelToActivateIn)
        {
            Debug.Log(string.Format("activation time & levelIndex correct. ACTIVATING! object = " + Helper.GetHierarchy(gameObject)));
            doStuff();
        }
    }

    void Update()
    {

        switch (fadeStatus)
        {
            case FadeStatus.FadingIn:
                if (this != BGMinstance && this != SFXinstance)     
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
                if (this != BGMinstance && this != SFXinstance) // if started fade out but another instance has since taken over, stop
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
        if (activationTime == ActivationTime.OnTrigger && otherCollider.tag == "player_tag" && !triggered)
        {   // only do stuff if set to operate on trigger
            triggered = true;
            doStuff();
        }
    }

    public void FadeMusicAndSFX()
    {
        // called from animation to fade both music and SFX levels
        // NOTE: for this to work, there should be two MenuMusicControllers on the same object,
        //       one with musicTopVol as the mixer and the other with sfxTopVol
        foreach (var controller in GetComponents<MenuMusicController>())
            controller.FadeOutVol(-80, 3);
    }

    public void ChangeClip()
    {
        bgMusic.clip = substituteClip;
    }

    public void CueMusic()
    {
        if (!bgMusic.isPlaying)
            bgMusic.Play();
    }

    public void FadeToVol()
    {
        FadeToVol(this.targetVolume, this.fadeTime);
    }
    public void FadeToVol(float targetVolume, float fadeTime)
    {
        float currVol;
        mainMixer.GetFloat(mixer.ToString(), out currVol);
        Debug.Log("FadeToVol() called");
        if (currVol == targetVolume)
            return;                                     // volume is already fine, do nothing
        else if (currVol > targetVolume)
            FadeOutVol(targetVolume, fadeTime);        // volume louder than target, fade out
        else
            FadeInVol(targetVolume, fadeTime);           // volume softer than target, fade in

    }

    public void FadeInVol()
    {
        FadeInVol(this.targetVolume, this.fadeTime);
    }
    public void FadeInVol(float targetVolume, float fadeTime)
    {
        //Debug.Log(string.Format("FadeInVol called with targetVol = {0}, fadeTime = {1}. mixer= {2}", targetVolume, fadeTime, mixer));
        this.targetVolume = targetVolume;
        this.fadeTime = fadeTime;

        if (!bgMusic.isPlaying)
            bgMusic.Play();

        float currVol;
        mainMixer.GetFloat(mixer.ToString(), out currVol);
        if (currVol > targetVolume) // if the volume is already higher than the target, do nothing
            return;

        switch (mixer)                      // set as appropriate active instance
        {
            case Mixer.musicTopVol:
                BGMinstance = this;
                break;
            case Mixer.sfxTopVol:
                SFXinstance = this;
                break;
        }
        fadeStatus = FadeStatus.FadingIn;
        counter = fadeTime;

        //get deltaVol (current volume - target)
        mainMixer.GetFloat(mixer.ToString(), out currVol);
        deltaVolume = Mathf.Abs((currVol - targetVolume) / fadeTime);

        mainMixer.GetFloat(mixer.ToString(), out currVol);


    }

    public void FadeOutVol()
    {
        FadeOutVol(targetVolume, fadeTime);
    }
    public void FadeOutVol(float targetVolume, float fadeTime)
    {
        //Debug.Log(string.Format("FadeOutVol called with targetVol = {0}, fadeTime = {1}. mixer= {2}", targetVolume, fadeTime, mixer));
        this.targetVolume = targetVolume;
        this.fadeTime = fadeTime;
        switch (mixer)                      // set as appropriate active instance
        {
            case Mixer.musicTopVol:
                BGMinstance = this;
                break;
            case Mixer.sfxTopVol:
                SFXinstance = this;
                break;
        }

        float currVol;
        mainMixer.GetFloat(mixer.ToString(), out currVol);

        //get deltaVol (current volume - target)
        mainMixer.GetFloat(mixer.ToString(), out currVol);
        deltaVolume = Mathf.Abs((currVol - targetVolume) / fadeTime);

        if (currVol < targetVolume)
        {
            Debug.Log(string.Format("Trying to fade out but current volume ({0}) is lower than target volume! ({1})", currVol, targetVolume));
            return;
        }
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
            mainMixer.SetFloat(mixer.ToString(), vol);
    }
    private void AddVolume(float volToAdd)
    {
        float currVol;
        mainMixer.GetFloat(mixer.ToString(), out currVol);
        mainMixer.SetFloat(mixer.ToString(), currVol + volToAdd);
    }
}
