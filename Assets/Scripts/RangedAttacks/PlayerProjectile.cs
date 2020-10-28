using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
   
   
    public Rigidbody2D rb;

    public void InitializeProjectile(Vector2 shooterPosition, float speed)
    {
        Vector2 direction = (Vector2)transform.position - shooterPosition;

        Debug.Log(direction);

        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MeleeEnemyController>().TakeDamage(10, transform.position, 3);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}

/*
   Could be used for magic projectile 

   public float speed;
   public float lifetime;

   //public GameObject destroyEffect;

   // Start is called before the first frame update
   private void Start()
   {
       //Invoke("DestroyProjectile", lifetime);
       Destroy(gameObject, lifetime);
   }

   // Update is called once per frame
   void Update()
   {
       transform.Translate(Vector2.right * speed * Time.deltaTime);
   }

   /* void DestroyProjectile()
   {
       Instantiate(destroyEffect, transform.position, Quaternion.identity);
       Destroy(gameObject);
   } */

