using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public GameObject player;

    private float zoom;
    private float distance;

    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        zoom = Input.GetAxis("Mouse ScrollWheel") * 10;
        Debug.Log("Camera Distance: " + distance);
        Debug.Log("Zoom: " + zoom);
        
        

        if (distance > 3 && distance < 15)
        {
            transform.position += transform.forward * zoom;
        }
        
        else if (distance < 3)
        {
            if (zoom < 0)
            {
                transform.position += transform.forward * zoom;
            }
        }

        else
        {
            if (zoom > 0)
            {
                transform.position += transform.forward * zoom;
            }
        }
    }
}
