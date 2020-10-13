using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHitbox : MonoBehaviour
{
    bool initialized = false;
    float lifespan = 0f;
    int damage;
    string spawner;

    // Update is called once per frame
    void Update()
    {
        if(initialized)
        {
            lifespan -= Time.deltaTime;
            if(lifespan <= 0)
            {
                Debug.Log("Destroying");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag == "Player" && spawner != "Player")
        {
            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (collision.tag == "Enemy" && spawner != "Enemy")
        {
            collision.GetComponent<MeleeEnemyController>().TakeDamage(damage);
        }
    }
    public void Initialize(string spawner, Vector2 size, Vector2 position, float duration, int damage)
    {
        BoxCollider2D hitbox = GetComponent<BoxCollider2D>();

        hitbox.size = size;
        hitbox.offset = position;
        hitbox.enabled = true;
        lifespan = duration;
        this.damage = damage;
        this.spawner = spawner;
        initialized = true;
    }
}
