using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MeleeEnemyController
{
    public GameObject fireball;
    public float slashSpeed = 15f;
    Vector2 targetPosition;

    GameObject spinHitBox;
    string mode = "Idle";

    float idleTime = 0f;

    bool enraged = false;
    // Update is called once per frame
    void Update()
    { 
        if(health <= 0)
        {
            return;
        }

        if (health < (maxHealth / 2))
        {
            //enter enrageMode
            enraged = true;
        }


        if(mode == "Idle") //boss is waiting between an attack
        {         
            //pick a new action
            if(idleTime <= 0)
            {
                ChooseAction();
            }
        }
        else if (mode == "Slash")
        {
            targetPosition = player.transform.position;
            float distance = Vector2.Distance(transform.position, targetPosition);
            if (distance <= attackRange) //we're close enough to the target position, so stop the attack
            {
                animator.SetTrigger("Slash");
                mode = "Idle";
                idleTime = 2f;
            }
            else
            {
                MoveToPoint(speed+(-1 * idleTime));
            }
        }
        else if (mode == "Spinning")
        {
            float distance = Vector2.Distance(transform.position, targetPosition);

            if(distance <= 1f) //we're close enough to the target position, so stop the attack
            {
                animator.SetBool("Spinning", false);
                animator.ResetTrigger("Hit");

                Destroy(spinHitBox);

                mode = "Idle";
                idleTime = 1.5f;
            }
            else //we still have to move closer
            {
                MoveToPoint(slashSpeed);
            }
            
        }
        else if (mode == "Magic") //face player and then do nothing (animation already playing)
        {
            targetPosition = player.transform.position;
            if (targetPosition.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            mode = "Idle";
            idleTime = 3f;
        }

        currentCooldown -= Time.deltaTime;
        invulnerabilityTimer -= Time.deltaTime;
        idleTime -= Time.deltaTime;

    }

    void MoveToPoint(float moveSpeed)
    {
        if (targetPosition.x - transform.position.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void ChooseAction()
    {
        int action = Random.Range(0, 3);
        Debug.Log("Chose: " + action);
        rb.velocity = Vector2.zero;
        animator.ResetTrigger("Hit");

        if(action == 0)
        {
            mode = "Slash";
        }
        else if (action == 1)
        {
            mode = "Spinning";
            animator.SetBool("Spinning", true);
            targetPosition = player.transform.position;

        }
        else if (action == 2)
        {
            mode = "Magic";
            animator.SetTrigger("Magic");
        }
        
    }

    public void SlashAttack()
    {
        animator.ResetTrigger("Hit");


        currentCooldown = attackCooldown;
        Vector3 spawnPosition = transform.position;

        spawnPosition += spriteRenderer.flipX ? transform.right * -1 : transform.right * 1;
        BasicHitbox hitBox = Instantiate(attackHitBox, spawnPosition, Quaternion.identity).GetComponent<BasicHitbox>();

        hitBox.Initialize("Enemy", new Vector2(1.5f, 1.5f), new Vector2(0, 0), .1f, 20, 2f);
    }

    public void SpinAttack() //boss targets where player currently is and twirls scythe moving towards that location
    {
        targetPosition = player.transform.position; //simply pick where the player is
        targetPosition = player.transform.position; //simply pick where the player is

        int bossBorderLayer = LayerMask.NameToLayer("BossRoomBorder");
        Vector2 relativePosition = player.transform.position - transform.position;
        relativePosition = relativePosition.normalized; //get vector for direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, relativePosition, 100, 1 << bossBorderLayer);

        Debug.DrawRay(transform.position, relativePosition, Color.green, 2f);

        if (hit.collider != null)
        {
            targetPosition = hit.point;
        }

        //Debug.DrawLine(transform.position, targetPosition, Color.red, 2f);
        mode = "Spinning";
        animator.SetBool("Spinning", true);

        BasicHitbox hitBox = Instantiate(attackHitBox, transform).GetComponent<BasicHitbox>();
        spinHitBox = hitBox.gameObject;
        hitBox.Initialize("Enemy", new Vector2(8f, 5f), new Vector2(0, 0), 100f, 20, 4f);
    }

    public void MagicAttack()
    {
        //Calculate relative position in order to spawn the projectile between the player and the enemy
        Vector2 spawnPosition = (Vector2)transform.position;
        Vector2 relativePosition = player.transform.position - transform.position;

        if (relativePosition.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        Quaternion projectileRotation = Quaternion.AngleAxis(Mathf.Atan2(relativePosition.y, relativePosition.x) * Mathf.Rad2Deg, Vector3.forward);

        relativePosition = relativePosition.normalized;

        //Spawn the projectile
        GameObject Projectile = Instantiate(fireball, (Vector2)transform.position + relativePosition, projectileRotation);

        //Initialize the projectile, giving it the information it needs
        Projectile.GetComponent<Projectile>().InitializeProjectile(transform.position, 20);
    }

    public void BossDeath()
    {
        DestroyMob();
    }
}
