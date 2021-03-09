using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHitbox : MonoBehaviour
{

    public int damage;
    public string spawner;
    public float knockbackForce;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player" && spawner != "Player")
        {

            if (knockbackForce != 0)
            {
                collision.GetComponent<PlayerController>().TakeDamage(damage, transform.position, knockbackForce);
            }
            else
            {
                collision.GetComponent<PlayerController>().TakeDamage(damage);
            }

        }
        else if (collision.tag == "Enemy" && spawner != "Enemy")
        {
            if (knockbackForce != 0)
            {
                collision.GetComponent<MeleeEnemyController>().TakeDamage(damage, transform.position, knockbackForce);
            }
            else
            {
                collision.GetComponent<MeleeEnemyController>().TakeDamage(damage);
            }
        }
    }
}
