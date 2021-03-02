using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTriggerArea : MonoBehaviour
{
    [SerializeField] private UI_Shop uiShop;
    [SerializeField] private Text bottomText;

    IShopCustomer shopCustomer = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (shopCustomer != null)
            {
                if (uiShop.gameObject.activeSelf)
                {
                    uiShop.Hide();
                    bottomText.text = "Open Shop 'E'";
                    bottomText.enabled = true;
                }
                else
                {
                    uiShop.Show(shopCustomer);
                    bottomText.enabled = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IShopCustomer customer = collision.GetComponent<IShopCustomer>();
        if (customer != null)
        {
            shopCustomer = customer;
            bottomText.text = "Open Shop 'E'";
            bottomText.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        shopCustomer = collision.GetComponent<IShopCustomer>();
        if (shopCustomer != null)
        {
            shopCustomer = null;
            uiShop.Hide();
            bottomText.enabled = false;
        }
    }
}
