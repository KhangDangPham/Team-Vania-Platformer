using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{

    // Use this for initialization
    [SerializeField] private GameObject CoinPrefab;
    public int coinCount = 10;

    public GameObject DropCoins()
    {
        GameObject newCoin = Instantiate(CoinPrefab, transform.position, Quaternion.identity) as GameObject;
        newCoin.GetComponent<CoinController>().count = coinCount;
        return newCoin;
    }
}
