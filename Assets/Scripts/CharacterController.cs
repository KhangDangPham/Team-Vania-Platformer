using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle all of the player's movement
public class CharacterController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;
    bool isJumping = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jumping");
            isJumping = true;
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


        //rb.AddForce(movement * speed * rb.mass * rb.drag);
    }
}
