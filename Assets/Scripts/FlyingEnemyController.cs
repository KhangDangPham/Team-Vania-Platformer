using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MeleeEnemyController
{
    public float movementRange = 10f;
    public float waitTime = .5f;

    int targetingMode = 0; //0 = random, 1 = player
    float currentWaitTime = 0f;
    Vector2 initialPoint;
    Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        initialPoint = transform.position;
        targetPosition = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = ChooseNewTargetPoint();

        if(currentWaitTime <= 0 && invulnerabilityTimer <= 0)
        {
            //face the enemy towards its movement position

            if (targetPosition.x - transform.position.x >= 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        currentCooldown -= Time.deltaTime;
        invulnerabilityTimer -= Time.deltaTime;
        currentWaitTime -= Time.deltaTime;
        HandleBlink();
        if (invulnerabilityTimer <= 0)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(10, transform.position, 2);
            currentWaitTime = 1;
        }
    }

    Vector2 ChooseNewTargetPoint()
    {
        Vector2 direction;
        int groundOnlyMask = LayerMask.GetMask("Ground");
        RaycastHit2D hit;
        Vector2 newPoint = Vector2.zero;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= aggroRange) //if we are in range of the player
        {
            newPoint = player.transform.position;

            //check if we can see the player

            direction = ((Vector2)transform.position - newPoint).normalized;
            hit = Physics2D.Raycast(transform.position, -direction, Vector2.Distance(newPoint, transform.position), groundOnlyMask);

            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawRay(transform.position, -direction, Color.blue, 1f);
                targetPosition = initialPoint;

            }
            else if( hit.collider == null) //we can see the player
            {
                Debug.DrawRay(transform.position, -direction, Color.red, 1f);
                targetingMode = 1;
                return newPoint;
            }
        }
        
        if(targetPosition == null || Vector2.Distance(transform.position, targetPosition) <= 0.5f)
        {
            //if we can't see the player, pick a random point nearby
            newPoint.x = Random.Range(-movementRange, movementRange);
            newPoint.y = Random.Range(-movementRange, movementRange);

            Debug.Log("New Point: " + newPoint);
            newPoint += initialPoint;

            direction = ((Vector2)transform.position - newPoint).normalized;
            hit = Physics2D.Raycast(transform.position, -direction, Vector2.Distance(newPoint, transform.position), groundOnlyMask);
            Debug.DrawRay(transform.position, -direction, Color.green, 1f);

            if (hit.collider != null)
            {
                targetingMode = 0;
                currentWaitTime = waitTime;
                return initialPoint;
            }
            else
            {
                targetingMode = 0;
                currentWaitTime = waitTime;
                return newPoint;
            }
        }
        else
        {
            return targetPosition;
        }
        
    }

    new public void TakeDamage(int damage, Vector2 enemyPosition, float force = 6f)
    {

        if (invulnerabilityTimer > 0f)
        {
            return;
        }

        TakeDamage(damage);

        if (health > 0)
        {
            invulnerabilityTimer = 1f;
            blinkMode = 1;
        }


        Vector2 kbMovement = (Vector2)transform.position - enemyPosition;

        if (kbMovement.x < 0)
        {
            kbMovement.x = -1;
        }

        kbMovement.y = 1;

        kbMovement *= force;

        rb.AddForce(kbMovement, ForceMode2D.Impulse);

        Debug.Log("setting trigger");
        animator.SetTrigger("Hit");
    }

}
