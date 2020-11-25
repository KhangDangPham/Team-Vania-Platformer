using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    //public float fallSpeed = 1f;
    public bool respawn = false;
    public float lifeTime;
    Rigidbody2D rb;
    float currentLife;
    Vector2 startPos;
 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        currentLife = lifeTime;
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
            rb.isKinematic = false;
     }
    // Update is called once per frame
    void Update()
    {   if (rb.isKinematic == false)
        {
           // transform.position += new Vector3(0, -fallSpeed * Time.deltaTime, 0);
            currentLife -= Time.deltaTime;
            if (currentLife <= 0)
            {
                if (respawn)
                {
                    rb.isKinematic = true;
                    transform.position = startPos;
                    currentLife = lifeTime;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
