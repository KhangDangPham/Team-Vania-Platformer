using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleHeart : MonoBehaviour
{
    public float minSize = 1.5f;
    public float maxSize = 2;
    public float moveDistance = 1f;
    public float growthSpeed;
    public float moveSpeed;

    bool growing = true;
    bool rising = true;
    float initialY;

    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().Heal(20);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(growing)
        {
            transform.localScale += new Vector3(growthSpeed * Time.deltaTime, growthSpeed * Time.deltaTime, 0);
        }
        else
        {
            transform.localScale -= new Vector3(growthSpeed * Time.deltaTime, growthSpeed * Time.deltaTime, 0);
        }

        if (transform.localScale.x < minSize)
        {
            growing = true;
        }
        else if (transform.localScale.x > maxSize)
        {
            growing = false;
        }     
        
        if (rising)
        {
            transform.localPosition += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        }
        else
        {
            transform.localPosition -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
        }

        if(transform.position.y > initialY +  moveDistance)
        {
            rising = false;
        }
        else if (transform.position.y < initialY - moveDistance)
        {
            rising = true;
        }
    }
}
