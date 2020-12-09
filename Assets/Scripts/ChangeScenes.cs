using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
    public static ChangeScenes instance = null;


    public Image black;
    public Animator anim;
    public string nextScene;

    private void Awake()
    {
        //Start at Intro Scene
        instance = this;
        if(SceneManager.GetActiveScene().name == "Team Intro Scene")
        {
            anim.SetTrigger("FadeOut");
            Invoke("loadStart", 2f);
         
        }
    }
  
   //Scene Changer
   public void ChangeScene(string sceneName)
    {
        anim.SetTrigger("FadeOut");
        SceneManager.LoadScene(sceneName);
    }
    //Load Start - Might be used later
    void loadStart()
    {
        anim.SetTrigger("FadeOut");
        ChangeScene(nextScene);
    }
    //Exit Game
    public void QuitGame()
    {
        Application.Quit();
    }

}
