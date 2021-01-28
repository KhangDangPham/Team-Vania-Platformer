﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script will handle all of the player's movement
public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 10;
    public int health = 100;
    public float mana = 100;

    public GameObject attackHitBox;
    public GameObject magicBurstPrefab;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    RangedCombat bowScript;
    HealthBar hpBar;
    RangedVisualController rangedVisual;

    bool canMove = true;
    bool canShoot = false;
    bool isJumping = false;
    bool hasJumped = false;
    int jumps = 2;
    float jumpTimer = 0f;
    int maxHealth;

    //used when the player gets hit
    float invulnerabilityTimer = 0f;
    int blinkMode = 0; //0 = no blinking, 1 = decreasing opacity, 2 = increasing opacity

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpBar = GetComponentInChildren<HealthBar>();
        rangedVisual = GetComponentInChildren<RangedVisualController>();

        hpBar.InitializeHealthBar(health);
        maxHealth = health;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump") && jumps > 0 && canMove)
        {

            if (!isJumping) //if not already jumping, call jump animation
            {
                animator.SetBool("IsJumping", true);
            }
            else //if we're already jumping, do double jump
            {
                animator.SetTrigger("DoubleJump");
            }
        }

        if(Input.GetKeyDown(KeyCode.Q) && mana >= 100)
        {
            invulnerabilityTimer = 4f;
            FindObjectOfType<AudioManager>().Play("MagicBurst"); //sfx
            animator.SetTrigger("Magic");
        }
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            Debug.Log(clipName);
            if (clipName != "slash" && clipName != "SpinAttack")
            {
                animator.SetTrigger("BasicAttack");
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(!canShoot)
            {
                animator.SetBool("IsAiming", true);
                canShoot = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(15);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            health = maxHealth;
            hpBar.UpdateHealth(health);
        }

        HandleBlink();
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

        if(!CheckSidesWalk(movement.x))
        {
            transform.position += new Vector3(movement.x, movement.y, 0) * speed * Time.deltaTime;
        }
        else
        {
            animator.SetBool("IsWalking", false);
            
        }
        

        RobustGrounded();

        invulnerabilityTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        mana = mana >= 100 ? 100 : mana + Time.deltaTime;
        hpBar.UpdateMana(mana);
    }

    bool Grounded(Vector3 startPos)
    {

        int groundOnlyLayer = LayerMask.NameToLayer("Ground"); //remove this if problems caused
        RaycastHit2D hit = Physics2D.Raycast(startPos, -Vector2.up, 100, 1 << groundOnlyLayer);

        if (hit.collider != null)
        {
            float distanceToGround = Mathf.Abs(hit.point.y - startPos.y);
            if(distanceToGround < .94)
            {
                if(hasJumped && jumpTimer < 0) //check for grounded after player has jumped
                {
                    animator.SetBool("IsJumping", false);
                    animator.ResetTrigger("DoubleJump");
                    isJumping = false;
                    hasJumped = false;
                    jumps = 2;
                }

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

    bool RobustGrounded()
    {
        if (Grounded(transform.position + new Vector3(.47f, 0, 0)))
        {
            return true;
        }
        else if (Grounded(transform.position))
        {
            return true;
        }
        else if(Grounded(transform.position + new Vector3(-.47f, 0, 0)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetGrounded()
    {
        animator.SetBool("IsJumping", false);
        animator.ResetTrigger("DoubleJump");
        isJumping = false;
        hasJumped = false;
        jumps = 2;
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce));
        FindObjectOfType<AudioManager>().Play("Jump"); //sfx
        jumps -= 1;
        jumpTimer = .4f;
        hasJumped = true;
        isJumping = true;

    }

    public void TakeDamage(int damage)
    {
        if (invulnerabilityTimer > 0)
        {
            return;
        }
        health -= damage;

        hpBar.UpdateHealth(health);

        if (health <= 0)
        {
            health = 0;
            Die();
            return;
        }

        invulnerabilityTimer = 2;
        blinkMode = 1;
        animator.SetTrigger("GetHit");
    }

    public void TakeDamage(int damage, Vector2 enemyPosition, float force = 6f)
    {
        if (invulnerabilityTimer > 0)
        {
            return;
        }

        FindObjectOfType<AudioManager>().Play("Hurt"); //sfx
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

    public void Heal(int amountHealed)
    {
        health += amountHealed;
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        hpBar.UpdateHealth(health);
    }

    bool CheckSidesWalk(float xMove)
    {
        int groundOnlyMask = LayerMask.GetMask("Ground");
        Vector3 shootPos = transform.position - new Vector3(0, .9f, 0);
        RaycastHit2D leftCheck = Physics2D.Raycast(shootPos, Vector2.left, 10f, groundOnlyMask);
        RaycastHit2D rightCheck = Physics2D.Raycast(shootPos, Vector2.right, 10f, groundOnlyMask);
        float distanceLeft = 100f;
        float distanceRight = 100f;

        if (leftCheck.collider != null)
        {
            distanceLeft = Mathf.Abs(leftCheck.point.x - transform.position.x);
        }

        if (rightCheck.collider != null)
        {
            distanceRight = Mathf.Abs(rightCheck.point.x - transform.position.x);
        }

        if (xMove < 0)
        {
            return distanceLeft <= .5f;
        }
        else
        {
            return distanceRight <= .5f;
        }
    }

    private void HandleBlink()
    {

        if(blinkMode == 0) //blink mode is 0 so we shouldn't be blinking
        {
            return;
        }

        Color tempColor = spriteRenderer.color;
        if (blinkMode == 1)
        {
            if(tempColor.a > .9)
            {
                blinkMode = 2;
            }
        }
        else if(blinkMode == 2)
        {
            if (tempColor.a < .2)
            {
                blinkMode = 1;
            }
        }

        tempColor.a += blinkMode == 1 ? 2*Time.deltaTime : -2*Time.deltaTime; //if blink mode is 1, we are decreasing opacity, if not, we are increasing opacity

        if(invulnerabilityTimer <= 0) //player is done being invulnerable
        {
            tempColor.a = 1;
            blinkMode = 0;
        }

        spriteRenderer.color = tempColor;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator playerDied()
    {
        Image faderImage = GameObject.Find("Fader").GetComponent<Image>();

        for (int i = 0; i <= 100; i++)
        {
            faderImage.color = new Color(0, 0, 0, (float)i / 100.0f);
            yield return new WaitForSeconds(0.015f);
        }
    }

    private void Die()
    {
        FindObjectOfType<AudioManager>().Play("Death"); //sfx
        animator.SetTrigger("Die");
        StartCoroutine(playerDied());
        canMove = false;
    }

    public void BasicMeleeAttack()
    {
        Vector3 spawnPosition = transform.position;

        spawnPosition += spriteRenderer.flipX ? transform.right * -1 : transform.right * 1;
        BasicHitbox hitBox = Instantiate(attackHitBox, spawnPosition, Quaternion.identity).GetComponent<BasicHitbox>();
        FindObjectOfType<AudioManager>().Play("Slash"); //sfx
        hitBox.Initialize("Player", new Vector2(2, 2), new Vector2(0, 0), .1f, 20, 3);
    }

    public void SpinAttack()
    {
        Vector3 spawnPosition = transform.position;

        BasicHitbox hitBox = Instantiate(attackHitBox, transform).GetComponent<BasicHitbox>();
        FindObjectOfType<AudioManager>().Play("SpinAtk"); //sfx
        hitBox.Initialize("Player", new Vector2(10, 7.5f), new Vector2(0, 0), .3f, 30, 5);
    }
    
    public void ResetAttack()
    {
        animator.ResetTrigger("BasicAttack");
    }

    public void MagicBurstAttack()
    {
        canMove = false;
        mana = 0;
        Instantiate(magicBurstPrefab, transform);
        animator.ResetTrigger("Magic");
    }

    public void CallShootMode()
    {
        if(canShoot)
        {
            canMove = false;
            rangedVisual.EnterShootMode();
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.ResetTrigger("BasicAttack");
        animator.SetBool("IsAiming", false);
        animator.ResetTrigger("Magic");
        canShoot = false;
    }
}
