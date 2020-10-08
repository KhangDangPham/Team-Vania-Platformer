using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTrap : MonoBehaviour
{
    public GameObject PoisonCloud;
    public float cooldown = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && cooldown <= 0)
        {
            SpawnPoison(transform.position);
        }
    }

    void SpawnPoison(Vector2 spawnPoint)
    {
        Instantiate(PoisonCloud, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        cooldown = 10;
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

}
