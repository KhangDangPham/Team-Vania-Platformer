using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudController : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerCoins playerCoins;
    public PlayerMana playerMana;

    [SerializeField] GameObject heartPrefab;
    [SerializeField] Image manaBar;
    [SerializeField] GameObject hearthBar;
    TextMeshProUGUI coinCountTxt;

    List<Image> hearts = new List<Image>();

    // Start is called before the first frame update
    void Start()
    {
        coinCountTxt = GameObject.Find("countTxt").GetComponent<TextMeshProUGUI>();
        InitializeHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateManaBar();
        coinCountTxt.text = playerCoins.numCoins.ToString();
    }

    public virtual void InitializeHealthBar()
    {
        int maxHealth = playerHealth.maxHealth;
        int numHearts = maxHealth / 20;

        if (maxHealth % 20 != 0)
        {
            numHearts += 1;
        }

        for (int i = 0; i < numHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, hearthBar.transform).GetComponent<Image>();
            newHeart.type = Image.Type.Filled;
            newHeart.fillMethod = Image.FillMethod.Radial360;
            newHeart.fillOrigin = 2;
            newHeart.fillClockwise = false;
            hearts.Add(newHeart);
        }

        UpdateHealthBar();
    }

    public virtual void UpdateHealthBar()
    {
        int currentHealth = playerHealth.currentHealth;
        int currentHeart = 0;

        ClearHealthBar();
        for (int i = currentHealth; i > 0; i-=20)
        {
            if (i >= 20)
            {
                hearts[currentHeart].fillAmount = 1;
                currentHeart++;
            }
            else if (i < 20 && i > 0)
            {
                hearts[currentHeart].fillAmount = ((float)i / 5) * (.25f);
            }

        }
    }

    public void UpdateManaBar()
    {
        manaBar.fillAmount = (float)playerMana.currentMana / playerMana.maxMana;
    }

    void ClearHealthBar()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].fillAmount = 0;
        }
    }
}
