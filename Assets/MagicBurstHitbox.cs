using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBurstHitbox : MonoBehaviour
{
    public float lifeSpan = 2f;
    public float increaseSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MeleeEnemyController>().TakeDamage(100, transform.position, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeSpan <= 0)
        {
            Destroy(gameObject);
        }
        lifeSpan -= Time.deltaTime;
        transform.localScale += new Vector3(increaseSpeed * Time.deltaTime, increaseSpeed * Time.deltaTime, 0);
    }
}
