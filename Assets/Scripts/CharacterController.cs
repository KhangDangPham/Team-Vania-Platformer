using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle all of the player's movement
public class CharacterController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;
    public int health = 100;
    public Vector2 respawnPoint;

    Rigidbody2D rb;

    bool isJumping = false;
    int jumps = 2;



    void Start()
    {
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && jumps > 0)
        {
            if (Grounded())
            {
                jumps = 1;
            }
            else
            {
                jumps = 0;
            }
            Debug.Log("Jumping");
            isJumping = true;
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(100);
        }

    }
    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = 0; //placeholder for other vertical forces minus jumping



        Vector2 movement = new Vector2(horizontalMove, verticalMove);

        transform.position += new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;

        if(isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce));
            isJumping = false;
        }
        Grounded();

        //rb.AddForce(movement * speed * rb.mass * rb.drag);
    }

    bool Grounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        if(hit.collider != null)
        {
            float distanceToGround = Mathf.Abs(hit.point.y - transform.position.y);
            Debug.Log("Hit: " + hit.collider.gameObject.name + "Distance to ground: " + distanceToGround);
            if(distanceToGround < .52)
            {
                jumps = 2;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        transform.position = respawnPoint;
        health = 100;
    }
}
