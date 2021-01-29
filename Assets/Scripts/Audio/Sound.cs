using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; //name of sound to ref
    public AudioClip clip;   

    public AudioMixerGroup group;

    [Range(0f,1f)]//volume range
    public float volume;
    [Range(.1f,3f)]//pitch range
    public float pitch;

    public bool loop;//make sound loop


    [HideInInspector]
    public AudioSource source;
}
