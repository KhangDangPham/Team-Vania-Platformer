using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle all of the player's movement
public class CharacterController : MonoBehaviour
{
    public float speed = 10;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalMove, verticalMove);

        rb.AddForce(movement * speed);
    }
}
