using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{

    Image hpBar;
    TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        hpBar = GetComponent<Image>();
        hpText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateHealth(int health, int maxHp)
    {
        hpBar.fillAmount = ((float)health / maxHp);
        hpText.text = health + "";
    }

}
