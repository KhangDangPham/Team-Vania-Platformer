using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

//This script will handle the player
public class PlayerController : MonoBehaviour, IShopCustomer
{
    public PlayerCoins playerCoins;
    public float playerSpeed = 2f;
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
    int jumps = 2;
    float jumpTimer = 0f;
    int maxHealth;

    float horizontalMove = 0f;
    [SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.

    //used when the player gets hit
    float invulnerabilityTimer = 0f;
    int blinkMode = 0; //0 = no blinking, 1 = decreasing opacity, 2 = increasing opacity

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private Vector3 velocity = Vector3.zero;

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
            animator.SetTrigger("Jump");
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Q) && mana >= 100)
        {
            invulnerabilityTimer = 4f;
            FindObjectOfType<AudioManager>().Play("MagicBurst"); //sfx
            animator.SetTrigger("Magic");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (clipName != "slash" && clipName != "SpinAttack")
            {
                animator.SetTrigger("BasicAttack");
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!canShoot)
            {
                animator.SetBool("IsAiming", true);
                canShoot = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(15);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            health = maxHealth;
            hpBar.UpdateHealth(health);
        }

        HandleBlink();

        horizontalMove = Input.GetAxisRaw("Horizontal") * playerSpeed;
    }
    void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        if (colliders.Length == 0)
            animator.SetBool("IsAerial", true);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && colliders[i].tag != "Unclimbable")
                m_Grounded = true;
        }

        if (canMove)
        {
            //flip the character to face the movement direction
            if (horizontalMove < 0)
            {
                spriteRenderer.flipX = true;
                animator.SetBool("IsWalking", true);
            }
            else if (horizontalMove > 0)
            {
                spriteRenderer.flipX = false;
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(horizontalMove * Time.fixedDeltaTime * 10f, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
        }
        else
        {
            horizontalMove = 0;
        }

        invulnerabilityTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        mana = mana >= 100 ? 100 : mana + Time.deltaTime;
        hpBar.UpdateMana(mana);
    }

    public void SetGrounded()
    {
        animator.SetBool("IsAerial", false);
        animator.ResetTrigger("Jump");
        jumps = 2;
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        FindObjectOfType<AudioManager>().Play("Jump"); //sfx
        jumps -= 1;
        jumpTimer = .4f;
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

        if (kbMovement.x < 0)
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
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        hpBar.UpdateHealth(health);
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
        if (canShoot)
        {
            canMove = false;
            rangedVisual.EnterShootMode();
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(0, rb.velocity.y);
        // And then smoothing it out and applying it to the character
        rb.velocity = new Vector2(0, 0); // Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.ResetTrigger("BasicAttack");
        animator.SetBool("IsAiming", false);
        animator.ResetTrigger("Magic");
        canShoot = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (m_Grounded && jumps == 0)
        {
            jumps = 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && animator.GetBool("IsAerial") == true || m_Grounded)
        {
            SetGrounded();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("IsAerial", true);
        }
    }

    public bool SpendCoins(int cost)
    {
        if (playerCoins.numCoins >= cost && cost >= 0)
        {
            playerCoins.numCoins -= cost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ObtainItem(Item.ItemType itemType)
    {
        Debug.Log("item type: " + itemType);
    }
}
