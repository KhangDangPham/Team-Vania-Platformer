using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MeleeEnemyController
{
    public GameObject projectile;
    public float projectileDamage = 10;

    protected override void Attack()
    {
        if (!ShotIsClear())
        {
            return;
        }

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

        relativePosition = relativePosition.normalized;
    
        //Spawn the projectile
        GameObject Projectile = Instantiate(projectile, (Vector2)transform.position + relativePosition, Quaternion.identity);

        //Initialize the projectile, giving it the information it needs
        Projectile.GetComponent<Projectile>().InitializeProjectile(transform.position, projectileDamage);
    }

    //Checks if the player is not blocked by any walls
    bool ShotIsClear()
    {
        Vector2 relativePosition = player.transform.position - transform.position;
        relativePosition = relativePosition.normalized;
        RaycastHit2D path = Physics2D.Raycast(transform.position, relativePosition);
        if(path.collider != null && path.collider.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
