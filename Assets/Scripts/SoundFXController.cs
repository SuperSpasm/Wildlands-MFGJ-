using UnityEngine;
using System.Collections;

public class SoundFXController : MonoBehaviour {
    public  AudioClip[] soundFX;
    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if (source.loop) // make sure looping is off
            source.loop = false;
    }

    public void playFX(string name)
    {
        AudioClip clipToPlay = null;
        foreach(AudioClip clip in soundFX)
        {
            if (clip.name == name)
            {
                clipToPlay = clip;
                break;
            }
        }
        if (clipToPlay)
        {
            source.clip = clipToPlay;
            source.Play();
        }
        else
            throw new System.ArgumentException("there is no clip by the name of " + name + " in the FX clip array!");
    }

    public void playFX(int index)
    {
        if (index < 0 || index > soundFX.Length - 1)
        {
            throw new System.IndexOutOfRangeException(string.Format("specified index ({0}) is out of range! array size: {1}", index, soundFX.Length));
        }
        else
        {
            source.clip = soundFX[index];
            source.Play();
        }
    }
}
