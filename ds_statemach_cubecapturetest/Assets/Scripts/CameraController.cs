using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float speed_h = 2.0f;
    public float speed_v = 2.0f;

    private float yaw;
    private float pitch;

    // Update is called once per frame
    void Update()
    {
        yaw += speed_h * Input.GetAxis("Mouse X");
        pitch -= speed_v * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void LateUpdate()
    {
        transform.position = player.transform.position;
    }
}
