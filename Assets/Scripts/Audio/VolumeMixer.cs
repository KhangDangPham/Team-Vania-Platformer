using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMixer : MonoBehaviour
{
    public string sourceName;
    public AudioMixer mixer;

    //Changes volume of given audio source 
    public void ChangeVolume(float sliderValue)
    {
        mixer.SetFloat(sourceName, Mathf.Log10(sliderValue) * 20);
    }
}
