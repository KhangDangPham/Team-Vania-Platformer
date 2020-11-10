using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    public void InitializeProjectile(Vector2 shooterPosition, float speed)
    {
        Vector2 direction = (Vector2)transform.position - shooterPosition;

        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10, transform.position, 3);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
