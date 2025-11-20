using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData")]
public class SoundData : ScriptableObject
{
    private static SoundData instance;

    public static SoundData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SoundData>("SoundData");
                if (instance == null)
                {
                    Debug.LogError("SoundData: Could not find SoundData asset in Resources folder.");
                }
            }
            return instance;
        }
    }
    
    public AudioSource SoundObject;

    public static void PlaySoundFXClip(AudioClip clip, Vector3 soundPos, float volume)
    {
        AudioSource a = Instantiate(Instance.SoundObject, soundPos, Quaternion.identity);
        a.clip = clip;
        a.volume = volume;
        a.Play();
    }

    public static void PlaySoundFXClip(AudioClip[] clips, Vector3 soundPos, float volume)
    {
        int randClip = Random.Range(0, clips.Length);
        AudioSource a = Instantiate(Instance.SoundObject, soundPos, Quaternion.identity);
        a.clip = clips[randClip];
        a.volume = volume;
        a.Play();
    }
}
