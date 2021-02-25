using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    public Rigidbody2D grappleRB;
    public SpringJoint2D joint;
    private bool launched;
    public bool contact;
    public float speed;
    public bool reachEnd;
    public GameObject destinationPoint;
    public LineRenderer rope;


    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine("startGrappleLife");

        rope.positionCount = 2;
        rope.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        rope.SetPosition(0, this.transform.position);
        rope.SetPosition(1, this.transform.parent.position);
    }

    public void startLaunch(Vector2 mouse)
    {
        destinationPoint.transform.position = mouse;
        //destinationPoint.SetActive(true);
        grappleRB.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        contact = true;
        rope.enabled = true;
        StopCoroutine("startGrappleLife");
        grappleRB.velocity = new Vector2(0f, 0f);
        grappleRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        joint.enabled = true;
    }

    public void ForceReturn()
    {
        returnToPlayer();
    }

    public void returnToPlayer()
    {
        
        contact = false;
        joint.enabled = false;
        this.transform.localPosition = new Vector3(0, 0, 0);
        grappleRB.constraints = RigidbodyConstraints2D.None;
        grappleRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        this.gameObject.SetActive(false);
        
    }

    IEnumerator startGrappleLife()
    {
        yield return new WaitForSeconds(.75f);
        Debug.Log("LIFE OVER");
        returnToPlayer();
    }

}
