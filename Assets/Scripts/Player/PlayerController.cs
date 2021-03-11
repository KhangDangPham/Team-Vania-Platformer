using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

//This script will handle the player
public class PlayerController : MonoBehaviour, IShopCustomer
{
    public Checkpoint checkpoint;
    public PlayerPosition playerPosition;
    public PlayerHealth playerHealth;
    public PlayerMana playerMana;
    public PlayerCoins playerCoins;
    public PlayerAbility playerAbility;

    public float playerSpeed = 2f;
    public float jumpForce = 10;

    public GameObject attackHitBox;
    public GameObject magicBurstPrefab;
    public GameObject Grapple;
    public GameObject attkHitbox;
    public GameObject fader;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    RangedCombat bowScript;
    RangedVisualController rangedVisual;
    BasicHitbox basicHitbox;

    bool canMove = true;
    bool canShoot = false;
    int jumps = 2;
    float jumpTimer = 0f;
    float elapsed_time = 0f;

    float horizontalMove = 0f;
    [SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    public bool m_Grounded;            // Whether or not the player is grounded.

    //used when the player gets hit
    float invulnerabilityTimer = 0f;
    int blinkMode = 0; //0 = no blinking, 1 = decreasing opacity, 2 = increasing opacity

    float blockTimer = 0f;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rangedVisual = GetComponentInChildren<RangedVisualController>();
        playerHealth.currentHealth = 100;
        basicHitbox = attkHitbox.GetComponent<BasicHitbox>();

        if(checkpoint.sceneName == SceneManager.GetActiveScene().name)
        {
            transform.position = checkpoint.position;
            
            playerHealth.currentHealth = checkpoint.currentHealth;
            playerHealth.maxHealth = checkpoint.maxHealth;

            playerMana.currentMana = checkpoint.currentMana;
            playerMana.maxMana = checkpoint.maxMana;

            playerCoins.numCoins = checkpoint.numCoins;

            playerAbility.shield = checkpoint.shield;
            playerAbility.grapple = checkpoint.grapple;
        }
    }

    private void Update()
    {
        playerPosition.position = rb.position;

        if (Input.GetButtonDown("Jump") && jumps > 0 && canMove)
        {
            animator.SetTrigger("Jump");
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Q) && playerMana.currentMana >= 100)
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
            playerHealth.currentHealth = playerHealth.maxHealth;
        }

        if (playerAbility.shield && Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Block");
        }

        if (playerAbility.grapple && Input.GetMouseButtonDown(2))
        {
            ShootGrapple();
        }

        HandleBlink();

        horizontalMove = Input.GetAxisRaw("Horizontal") * playerSpeed;
    }
    void FixedUpdate()
    {
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        if (colliders.Length == 0)
        {
            animator.SetBool("IsAerial", true);
            m_Grounded = false;
        }
        else
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && colliders[i].tag != "Unclimbable")
                {
                    m_Grounded = true;
                    SetGrounded();
                }
            }
        }

        if (canMove)
        {
            //flip the character to face the movement direction
            if (horizontalMove < 0)
            {
                FaceLeft();
                animator.SetBool("IsWalking", true);
            }
            else if (horizontalMove > 0)
            {
                FaceRight();
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
        blockTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;
        elapsed_time += Time.deltaTime;

        if (elapsed_time >= 1)
        {
            elapsed_time = 0f;
            playerMana.currentMana = playerMana.currentMana >= playerMana.maxMana ? playerMana.maxMana : playerMana.currentMana + 1;
        }
    }

    public void FaceRight()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
    }

    public void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Math.Abs(scale.x) * -1, scale.y, scale.z);
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
        if (playerHealth.currentHealth <= 0 || invulnerabilityTimer > 0 || blockTimer > 0)
        {
            return;
        }
        playerHealth.currentHealth -= damage;

        if (playerHealth.currentHealth <= 0)
        {
            playerHealth.currentHealth = 0;
            Die();
            return;
        }

        invulnerabilityTimer = 2;
        blinkMode = 1;
        animator.SetTrigger("GetHit");
    }

    public void TakeDamage(int damage, Vector2 enemyPosition, float force = 6f)
    {
        if (invulnerabilityTimer > 0 || blockTimer > 0)
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
        playerHealth.currentHealth += amountHealed;
        if (playerHealth.currentHealth > playerHealth.maxHealth)
        {
            playerHealth.currentHealth = playerHealth.maxHealth;
        }
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
        fader.SetActive(true);
        Image faderImage = fader.GetComponent<Image>();

        for (int i = 0; i <= 100; i++)
        {
            faderImage.color = new Color(0, 0, 0, (float)i / 100.0f);
            yield return new WaitForSeconds(0.015f);
        }
        fader.SetActive(false);
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
        basicHitbox.damage = 20;
        basicHitbox.knockbackForce = 3f;
        FindObjectOfType<AudioManager>().Play("Slash"); //sfx
    }

    public void SpinAttack()
    {
        basicHitbox.damage = 30;
        basicHitbox.knockbackForce = 5;
        FindObjectOfType<AudioManager>().Play("SpinAtk"); //sfx
    }

    public void ResetAttack()
    {
        attkHitbox.SetActive(false);
        animator.ResetTrigger("BasicAttack");
    }

    public void MagicBurstAttack()
    {
        canMove = false;
        playerMana.currentMana = 0;
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

    public void ShootGrapple()
    {
        if (Grapple.activeSelf == false)
        {

            Vector2 target = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;
            Grapple.transform.eulerAngles = new Vector3(0, 0, -angle - 90);
            Grapple.SetActive(true);
            Grapple.GetComponent<PlayerGrapple>().startLaunch(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }
        else
        {
            Grapple.GetComponent<PlayerGrapple>().ForceReturn();
        }

        //Physics.Raycast(mousePosition, )
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

    public bool SpendCoins(int cost, Item.ItemType itemType)
    {
        if (playerCoins.numCoins >= cost && cost >= 0 && !HaveItem(itemType))
        {
            playerCoins.numCoins -= cost;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HaveItem(Item.ItemType itemType)
    {
        if (itemType == Item.ItemType.Shield && playerAbility.shield)
        {
            return true;
        }
        else if (itemType == Item.ItemType.Grapple && playerAbility.grapple)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ObtainItem(Item.ItemType itemType)
    {
        if (itemType == Item.ItemType.Shield)
        {
            playerAbility.shield = true;
        }
        else if (itemType == Item.ItemType.Grapple)
        {
            playerAbility.grapple = true;
        }
    }

    public void ActivateBlock()
    {
        //DisableMovement();
        blockTimer = 1f;
    }

    public void DeactivateBlock()
    {
        //EnableMovement();
        blockTimer = 0f;
    }
}
