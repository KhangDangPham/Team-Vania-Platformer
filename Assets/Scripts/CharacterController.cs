using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle all of the player's movement
public class CharacterController : MonoBehaviour
{
    public float speed = 10;
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
        float verticalMove = 0; //placeholder for if we ever need vertical movement besides jumping

        if(isJumping)
        {
            verticalMove = 20;
            isJumping = false;
        }

        Debug.Log("VMove: " + verticalMove + " HMove: " + horizontalMove);

        Vector2 movement = new Vector2(horizontalMove, verticalMove);

        rb.AddForce(movement * speed * rb.mass);
    }
}
