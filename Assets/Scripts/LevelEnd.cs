using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEnd : MonoBehaviour
{
    public string nextScene;
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetTrigger("FadeOut");
        SceneManager.LoadScene(nextScene);
    }

}
