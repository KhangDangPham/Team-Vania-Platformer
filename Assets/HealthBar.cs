using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

    Image hpBar;
    Text hpText;

    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponent<Image>();
        hpText = GetComponentInChildren<Text>();
    }

    public void UpdateHealth(int health)
    {
        hpBar.fillAmount = ((float)health / 100);
        hpText.text = health + " / 100";
    }

}
