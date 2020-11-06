using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script will handle all of the player's movement
public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;
    public int health = 100;

    public GameObject attackHitBox;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    RangedCombat bowScript;
    HealthBar hpBar;

    float invulnerabilityTimer = 0f;
    bool canMove = true;
    bool isJumping = false;
    //bool hasLeftGround = false;
    int jumps = 2;
    int maxHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpBar = GetComponentInChildren<HealthBar>();
        bowScript = GetComponentInChildren<RangedCombat>();
        bowScript.SetPlayerTransform(transform);

        maxHealth = health;
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
            /*animator.SetBool("IsJumping", true);
            if (isJumping)
            {
                animator.SetTrigger("DoubleJump");
            }*/
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetTrigger("BasicAttack");
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            bowScript.Shoot();
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

        if(canMove)
        {
            //flip the character to face the movement direction
            if (horizontalMove < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (horizontalMove > 0)
            {
                spriteRenderer.flipX = false;
            }

            //Set walking variable
            if (horizontalMove != 0 && rb.velocity.x == 0)
            {
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            horizontalMove = 0;
        }
        

        Vector2 movement = new Vector2(horizontalMove, verticalMove);

        transform.position += new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;

        if(isJumping)
        {
            Jump();
        }

        Grounded();

        invulnerabilityTimer -= Time.deltaTime;
    }

    bool Grounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
        if(hit.collider != null)
        {
            float distanceToGround = Mathf.Abs(hit.point.y - transform.position.y);
            //Debug.Log("Hit: " + hit.collider.gameObject.name + "Distance to ground: " + distanceToGround);
            if(distanceToGround < .94)
            {
                jumps = 2;
                /*if (hasLeftGround)
                {
                    animator.SetBool("IsJumping", false);
                    isJumping = false;
                    hasLeftGround = false;
                }*/
                return true;
            }
            else
            {
                //hasLeftGround = true;
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce));
        jumps -= 1;
        isJumping = false;
    }

    public void TakeDamage(int damage)
    {
        if (invulnerabilityTimer > 0)
        {
            return;
        }
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
        invulnerabilityTimer = 2;
        animator.SetTrigger("GetHit");
    }

    public void TakeDamage(int damage, Vector2 enemyPosition, float force = 6f)
    {
        if (invulnerabilityTimer > 0)
        {
            return;
        }

        TakeDamage(damage);       

        Vector2 kbMovement = (Vector2)transform.position - enemyPosition;

        if(kbMovement.x < 0)
        {
            kbMovement.x = -1;
        }

        kbMovement.y = 1;

        kbMovement *= force;

        DisableMovement();

        rb.AddForce(kbMovement, ForceMode2D.Impulse);
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        health = 100;
    }

    public void BasicMeleeAttack()
    {
        Vector3 spawnPosition = transform.position;

        spawnPosition += spriteRenderer.flipX ? transform.right * -1 : transform.right * 1;
        BasicHitbox hitBox = Instantiate(attackHitBox, spawnPosition, Quaternion.identity).GetComponent<BasicHitbox>();
        
        hitBox.Initialize("Player", new Vector2(2, 2), new Vector2(0, 0), .25f, 15, 4);
    }

    public void DisableMovement()
    {
        canMove = false;
        
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.ResetTrigger("BasicAttack");
    }
}
