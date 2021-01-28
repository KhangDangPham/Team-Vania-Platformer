using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    Camera camera;
    public Transform target;
    public float smoothing;
    public float offset;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }
    void FixedUpdate()
    {
        if (target != null && transform.position != target.position)
        {
            Vector3 newPosition = new Vector3(target.position.x, target.position.y + offset, transform.position.z);
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
}
