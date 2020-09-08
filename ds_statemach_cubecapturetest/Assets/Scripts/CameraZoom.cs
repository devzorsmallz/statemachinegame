using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public bool canZoom = true;
    public float zoom;
    public float distance;
    public GameObject player;

    void Update()
    {
        // Get the distance between the player and the camera
        distance = Vector3.Distance(player.transform.position, transform.position);
        // Get the input from the scrollwheel
        zoom = Input.GetAxis("Mouse ScrollWheel") * 10;

        // If the distance between the player and the camera is between 6 and 15, zooming in and out works
        if (canZoom && distance > 6 && distance < 15)
        {
            transform.position += transform.forward * zoom;
        }
        

        // If the distance betwen the player and the camera is less than 6, only zooming out works
        else if (canZoom && distance < 6)
        {
            if (zoom < 0)
            {
                transform.position += transform.forward * zoom;
            }
        }


        // If the distance between the player and the camera is greater than 15, only zooming in works
        else
        {
            if (canZoom && zoom > 0)
            {
                transform.position += transform.forward * zoom;
            }
        }
    }
}
