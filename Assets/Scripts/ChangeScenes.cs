using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
    public static ChangeScenes instance = null;

    private void Awake()
    {
        //Start at Intro Scene
        instance = this;
        if(SceneManager.GetActiveScene().name == "Team Intro Scene")
        {
            Invoke("loadStart", 5f);
        }
    }
   
   //Scene Changer
   public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    //Load Start - Might be used later
    void loadStart()
    {
        ChangeScene("Start Menu");
    }
    //Exit Game
    public void QuitGame()
    {
        Application.Quit();
    }
}
