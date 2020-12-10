using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    public bool moving = true;
    Vector3 nextPos;

    // Start is called before the first frame update
    void Start()
    {
        if(moving)
        {
            nextPos = startPos.position;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.transform.SetParent(transform);
            if(collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().SetGrounded();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {

            if (transform.position == pos1.position)
            {
                nextPos = pos2.position;
            }
            if (transform.position == pos2.position)
            {
                nextPos = pos1.position;
            }

            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if(moving)
        {
            Gizmos.DrawLine(pos1.position, pos2.position);
        }
    }
}
