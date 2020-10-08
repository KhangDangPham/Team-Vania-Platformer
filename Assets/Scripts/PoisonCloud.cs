using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float lifeTime = 10f;
    public BoxCollider2D cloudZone;

    private void Start()
    {
        cloudZone = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entered");
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(25);
        }
    }

    void Update()
    {
        if((int)lifeTime % 2 == 0)
        {
            Debug.Log("Disabling");
            cloudZone.enabled = false;
        }
        else
        {
            Debug.Log("Enabling");
            cloudZone.enabled = true;
        }
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
