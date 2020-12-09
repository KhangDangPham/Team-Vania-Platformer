using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MeleeEnemyController
{
    public GameObject fireball;
    public float slashSpeed = 15f;
    Vector2 targetPosition;

    string mode = "Idle";

    float idleTime = 0f;

    // Update is called once per frame
    void Update()
    { 
        if(mode == "Idle") //boss is waiting between an attack
        {
            
            //pick a new action
            if(idleTime <= 0)
            {
                ChooseAction();
            }
            else
            {
                idleTime -= Time.deltaTime;
            }

        }
        else if (mode == "Spinning")
        {
            float distance = Vector2.Distance(transform.position, targetPosition);

            if(distance <= 1f) //we're close enough to the target position, so stop the attack
            {
                mode = "Idle";
                idleTime = 2f;
            }
            else //we still have to move closer
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, slashSpeed * Time.deltaTime);
            }
            
        }
        else if (mode == "Fireball")
        {

        }

        
    }

    void ChooseAction()
    {
        rb.velocity = Vector2.zero;

        mode = "Spinning";
        targetPosition = player.transform.position;

        if (targetPosition.x - transform.position.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    protected override void Attack()
    {
        animator.SetBool("IsWalking", false);
        animator.SetTrigger("Attack");
    }

    public void BasicAttack() //spawn basic melee attack
    {
        animator.ResetTrigger("Attack");
        currentCooldown = attackCooldown;
        Vector3 spawnPosition = transform.position;

        spawnPosition += spriteRenderer.flipX ? transform.right * 1 : transform.right * -1;
        BasicHitbox hitBox = Instantiate(attackHitBox, spawnPosition, Quaternion.identity).GetComponent<BasicHitbox>();

        hitBox.Initialize("Enemy", new Vector2(1.5f, 1.5f), new Vector2(0, 0), .1f, 25, 2.5f);
    }

    public void SpinAttack() //boss targets where player currently is and twirls scythe moving towards that location
    {
        targetPosition = player.transform.position;
        mode = "Spinning";

        BasicHitbox hitBox = Instantiate(attackHitBox, transform).GetComponent<BasicHitbox>();
        hitBox.Initialize("Enemy", new Vector2(1.5f, 1.5f), new Vector2(0, 0), .1f, 25, 2.5f);
    }

    public void MagicAttack()
    {
        animator.ResetTrigger("Attack");
        currentCooldown = attackCooldown;
        //Calculate relative position in order to spawn the projectile between the player and the enemy
        Vector2 spawnPosition = (Vector2)transform.position;
        Vector2 relativePosition = player.transform.position - transform.position;

        if (relativePosition.x > 0)
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
}
