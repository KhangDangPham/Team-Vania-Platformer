using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedCombat : MonoBehaviour
{

    public GameObject projectile;

    //public float offset;
    public float launchForce;

    public Transform startPoint;

    private float fireRate;
    private float hideCountdown = 0;
    public float startFireRate;
    public float hideDelay = 2.5f;

    SpriteRenderer spriteRenderer;
    Transform playerTransform;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        AdjustBowPosition();

        fireRate -= Time.deltaTime;
        hideCountdown -= Time.deltaTime;

        if(hideCountdown <= 0)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void SetPlayerTransform(Transform sentTransform)
    {
        playerTransform = sentTransform;
    }

    public void Shoot()
    {
        if (fireRate <= 0)
        {
            spriteRenderer.enabled = true;

            if (fireRate <= 0)
            {
                GameObject arrow = Instantiate(projectile, startPoint.position, startPoint.rotation);
                arrow.GetComponent<Rigidbody2D>().velocity = transform.right * launchForce;
                fireRate = startFireRate;
                hideCountdown = hideDelay;
            }
        }
    }

    private void AdjustBowPosition()
    {
        Vector2 objectPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - objectPosition;
        transform.right = direction;

        Vector2 posDifference = (Vector2)playerTransform.position - mousePosition;

        if(posDifference.x > 0.6f)
        {
            transform.localPosition = new Vector2(-3, transform.localPosition.y);
        }
        else if (posDifference.x < -0.6f)
        {
            transform.localPosition = new Vector2(3, transform.localPosition.y);
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
