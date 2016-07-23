using UnityEngine;
using System.Collections;

public class SoundFXController : MonoBehaviour {
    public  AudioClip[] soundFX;
    private AudioSource source;


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
    public void playFX() { }

    void Awake()
    {
        source = GetComponent<AudioSource>();

        if (source.loop) // make sure looping is off
            source.loop = false;
    }
    void Start()
    {

    }


}
