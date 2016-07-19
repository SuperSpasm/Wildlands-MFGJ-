using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetAudioLevels : MonoBehaviour {

	public AudioMixer mainMixer;					//Used to hold a reference to the AudioMixer mainMixer
    public AnimationCurve musicVolCurve;               // an nimation curve for mapping slider input to music volume. starts at (0,0), so input must be increased by 80 before evaluation
    public AnimationCurve sfxVolCurve;                 // an nimation curve for mapping slider input to sfx volume. starts at (0,0), so input must be increased by 80 before evaluation

    //Call this function and pass in the float parameter musicLvl to set the volume of the AudioMixerGroup Music in mainMixer
    public void SetMusicLevel(float musicLvl)
	{
        // adjust musicVol to curve
        musicLvl = musicVolCurve.Evaluate(musicLvl + 80);
		mainMixer.SetFloat("musicVol", musicLvl);
	}

	//Call this function and pass in the float parameter sfxLevel to set the volume of the AudioMixerGroup SoundFx in mainMixer
	public void SetSfxLevel(float sfxLevel)
	{
        // adjust sfxVol to curve
    
        mainMixer.SetFloat("sfxVol", sfxLevel);
	}
}
