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
    public float startFireRate;

    // Update is called once per frame
    private void Update()
    {
        /*Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
        */
        Vector2 objectPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - objectPosition;
        transform.right = direction;



        if (fireRate <= 0)
        {

            if (Input.GetMouseButtonDown(1))
            {
                GameObject arrow = Instantiate(projectile, startPoint.position, startPoint.rotation);
                arrow.GetComponent<Rigidbody2D>().velocity = transform.right * launchForce;
                fireRate = startFireRate;
            }

        }
        else
        {
            fireRate -= Time.deltaTime;
        }
    }
}
