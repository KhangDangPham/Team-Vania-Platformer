using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    Rigidbody2D rb;
    public float fallSpeed = 1f;
    public float lifeTime = 2f;
    bool falling = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
            rb.isKinematic = false;
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
            Debug.Log("You Lose");
    }

    // Update is called once per frame
    void Update()
    {
        if(!falling)
        {
            RaycastHit2D downCheck = Physics2D.Raycast(transform.position, transform.up);
            Debug.DrawRay(transform.position, transform.up * 5, Color.red, 1000);
            if (downCheck.collider != null)
            {
                if (downCheck.collider.tag == "Player")
                {
                    falling = true;
                }
            }
        }
        else
        {
            transform.position += new Vector3(0, -fallSpeed*Time.deltaTime, 0);
            lifeTime -= Time.deltaTime;
            if(lifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
