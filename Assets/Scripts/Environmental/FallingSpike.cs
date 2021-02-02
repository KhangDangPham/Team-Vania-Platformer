using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    public int distance = 5;
    public bool respawn = false;
    public float fallSpeed = 1f;
    public float lifeTime = 2f;
    float currentLifetime;
    bool falling = false;
    Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        currentLifetime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!falling)
        {
            RaycastHit2D downCheck = Physics2D.Raycast(transform.position, -transform.up);
            Debug.DrawRay(transform.position, -transform.up * distance, Color.red, 1000);
            if (downCheck.collider != null)
            {
                if (downCheck.collider.tag == "Player")
                {
                    falling = true;
                }
            }
        }
        else
        {
            transform.position += new Vector3(0, -fallSpeed * Time.deltaTime, 0);
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0)
            {
                if (respawn)
                {
                    transform.position = startPos;
                    falling = false;
                    currentLifetime = lifeTime;
                }
                else
                {
                    Destroy(gameObject);
                }

            }
        }
    }
}
