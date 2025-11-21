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
    
    public GameObject SoundObjectPrefab;

    public static void PlaySoundFXClip(AudioClip clip, Vector3 soundPos, float volume)
    {
        GameObject soundObj = Instantiate(Instance.SoundObjectPrefab, soundPos, Quaternion.identity);
        AudioSource a = soundObj.GetComponent<AudioSource>();
        a.clip = clip;
        a.volume = volume;
        a.Play();
        Destroy(soundObj, clip.length);
    }

    public static void PlaySoundFXClip(AudioClip[] clips, Vector3 soundPos, float volume)
    {
        int randClip = Random.Range(0, clips.Length);
        GameObject soundObj = Instantiate(Instance.SoundObjectPrefab, soundPos, Quaternion.identity);
        AudioSource a = soundObj.GetComponent<AudioSource>();
        a.clip = clips[randClip];
        a.volume = volume;
        a.Play();
        Destroy(soundObj, clips[randClip].length);
    }
}
