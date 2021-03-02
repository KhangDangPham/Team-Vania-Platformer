using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerPosition playerPosition;

    Camera camera;
    public Transform target;
    public float smoothing;
    public float offset;
    private Vector3 newPosition;
    private bool isLowering;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        if (target != null && transform.position != target.position)
        {
            
           

            if (!target.gameObject.GetComponent<PlayerController>().m_Grounded && target.GetComponent<Rigidbody2D>().velocity.y < 0 && !isLowering)
            {
                //StartCoroutine("lowerCamera");
            }

            newPosition = new Vector3(target.position.x, target.position.y + offset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothing);
        }
        else
        {
            Vector3 newPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, newPosition, smoothing);
        }
    }

    // Update is called once per frame
    void Update()
    {
        camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 2;
        if(camera.orthographicSize > 10)
        {
            camera.orthographicSize = 10;
        }
        else if(camera.orthographicSize < 3)
        {
            camera.orthographicSize = 3;
        }
    }

    IEnumerator lowerCamera()
    {
        isLowering = true;
        yield return new WaitForSeconds(0.15f);
        while(!target.gameObject.GetComponent<PlayerController>().m_Grounded && offset>0)
        {
            
            offset = offset-Time.deltaTime;
            Debug.Log(offset);
        }
        offset = 2.5f;
        isLowering = false;
    }
}
