using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    public Image barFill;

    int maxHealth;
    public override void InitializeHealthBar(int maxHp)
    {
        maxHealth = maxHp;
        currentHp = maxHealth;

        UpdateHealth(currentHp);
    }

    public override void UpdateHealth(int health)
    {
        currentHp = health;
        float fillAmt = (float)currentHp / (float)maxHealth;
        barFill.fillAmount = fillAmt;
        
    }

}
