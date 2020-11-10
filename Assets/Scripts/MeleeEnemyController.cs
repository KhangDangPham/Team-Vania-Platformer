﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{

    public int health = 100;

    public int damage = 25;
    public float aggroRange = 10f;
    public float speed = 1f;
    public float attackRange = 1.1f;
    public float attackCooldown = 1f;

    protected SpriteRenderer spriteRenderer;
    protected PlayerController player;
    protected Rigidbody2D rb;
    protected HealthBar hpBar;
    protected float currentCooldown = 0f;
    protected float jumpCooldown = 0f;
    protected float invulnerabilityTimer = 0f;
    protected int blinkMode = 0;
    protected int maxHealth;

    void Start()
    {
        //Looks for a gameobject with the tag "Player", and gets the PlayerController script
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpBar = GetComponentInChildren<HealthBar>();
        hpBar.UpdateHealth(health, maxHealth);

        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector2.Distance(transform.position, player.gameObject.transform.position);

        Vector2 positionDifference = player.gameObject.transform.position - transform.position;

        //if within aggro range but not within attack range
        if (attackRange < distance && distance <= aggroRange && invulnerabilityTimer <= 0)
        {

            //move towards player
            Vector2 movement = Vector2.zero;

            if(positionDifference.x > 0)
            {
                movement.x = 1;
                spriteRenderer.flipX = true;
            }
            else
            {
                movement.x = -1;
                spriteRenderer.flipX = false;
            }

            transform.position += new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;
        }
        else if(distance <= attackRange && currentCooldown <= 0 && invulnerabilityTimer <= 0) //attack
        {
            Attack();
        }

        Vector2 sideDistances = CheckSides();

        if((positionDifference.x < 0 && sideDistances[0] < 2) && jumpCooldown <= 0)
        {
            Jump();
        }
        else if((positionDifference.x > 0 && sideDistances[1] < 2) && jumpCooldown <= 0)
        {
            Jump();
        }

        currentCooldown -= Time.deltaTime;
        jumpCooldown -= Time.deltaTime;
        invulnerabilityTimer -= Time.deltaTime;
        HandleBlink();
    }

    Vector2 CheckSides()
    {
        int groundOnlyMask = LayerMask.GetMask("Ground");
        RaycastHit2D leftCheck = Physics2D.Raycast(transform.position, Vector2.left, 10f, groundOnlyMask);
        RaycastHit2D rightCheck = Physics2D.Raycast(transform.position, Vector2.right, 10f, groundOnlyMask);

        float distanceLeft = 100f;
        float distanceRight = 100f;

        if(leftCheck.collider != null)
        {
            distanceLeft = Mathf.Abs(leftCheck.point.x - transform.position.x);
        }

        if(rightCheck.collider != null)
        {
            distanceRight = Mathf.Abs(rightCheck.point.x - transform.position.x);
        }

        Vector2 distances = new Vector2(distanceLeft, distanceRight);

        return distances;
    }

    void Jump()
    {
        jumpCooldown = 1f;
        rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
            return;
        }
        else
        {
            hpBar.UpdateHealth(health, maxHealth);
        }

    }

    public void TakeDamage(int damage, Vector2 enemyPosition, float force = 6f)
    {

        if(invulnerabilityTimer > 0f)
        {
            return;
        }

        TakeDamage(damage);

        invulnerabilityTimer = 1.5f;
        blinkMode = 1;

        Vector2 kbMovement = (Vector2)transform.position - enemyPosition;

        if (kbMovement.x < 0)
        {
            kbMovement.x = -1;
        }

        kbMovement.y = 1;

        kbMovement *= force;

        rb.AddForce(kbMovement, ForceMode2D.Impulse);
       
    }

    private void HandleBlink()
    {

        if (blinkMode == 0) //blink mode is 0 so we shouldn't be blinking
        {
            return;
        }

        Color tempColor = spriteRenderer.color;
        if (blinkMode == 1)
        {
            if (tempColor.a > .9)
            {
                blinkMode = 2;
            }
        }
        else if (blinkMode == 2)
        {
            if (tempColor.a < .2)
            {
                blinkMode = 1;
            }
        }

        tempColor.a += blinkMode == 1 ? 2 * Time.deltaTime : -2 * Time.deltaTime; //if blink mode is 1, we are decreasing opacity, if not, we are increasing opacity

        if (invulnerabilityTimer <= 0) //player is done being invulnerable
        {
            tempColor.a = 1;
            blinkMode = 0;
        }

        spriteRenderer.color = tempColor;
    }

    protected virtual void Attack()
    {
        currentCooldown = attackCooldown;
        player.TakeDamage(damage, transform.position);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
