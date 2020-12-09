using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invisibility : MonoBehaviour
{
    //BoxCollider2D box;
    SpriteRenderer rend;
    [SerializeField]
    private bool pass = false;
    // Start is called before the first frame update
    void Start()
    {
        //box = this.gameObject.GetComponent<BoxCollider2D>();
        rend = this.gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            //pass = true;
            if (pass)
            {
                rend.enabled = false;
                //box.enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
