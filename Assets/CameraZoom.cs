using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
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
