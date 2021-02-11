using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHitbox : MonoBehaviour
{
    bool initialized = false;
    float lifespan = 0f;
    int damage;
    string spawner;
    float knockbackForce;
    

    // Update is called once per frame
    void Update()
    {
        if(initialized)
        {
            lifespan -= Time.deltaTime;
            if(lifespan <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag == "Player" && spawner != "Player")
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
    public void Initialize(string spawner, Vector2 size, Vector2 position, float duration, int damage, float kbForce = 0f)
    {
        BoxCollider2D hitbox = GetComponent<BoxCollider2D>();

        hitbox.size = size;
        hitbox.offset = position;
        hitbox.enabled = true;
        lifespan = duration;
        this.damage = damage;
        this.spawner = spawner;
        knockbackForce = kbForce;

        initialized = true;
    }
}
