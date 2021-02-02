using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEnd : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LevelChanger levelChanger = GameObject.Find("LevelChanger").GetComponent<LevelChanger>();
            levelChanger.GoToScene(sceneName);
            levelChanger.FadeToLevel(sceneName);
        }

    }

}
