using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField]
    public int damage = 100;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MeleeEnemyController>().TakeDamage(100);
        }
    }
}
