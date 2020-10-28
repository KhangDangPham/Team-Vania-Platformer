using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedCombat : MonoBehaviour
{

    public float offset;    
    public GameObject projectile;
    public Transform startPoint;
    private float fireRate;
    public float startFireRate;

    // Update is called once per frame
    private void Update()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);

        if (fireRate <= 0)
        {

            if (Input.GetMouseButtonDown(1))
            {
                GameObject arrow = Instantiate(projectile, startPoint.position, transform.rotation);
                arrow.GetComponent<PlayerProjectile>().InitializeProjectile(transform.position, 30);
                fireRate = startFireRate;
            }

        }
        else
        {
            fireRate -= Time.deltaTime;
        }
    }
}
