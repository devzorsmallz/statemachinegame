using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public int captureTime = 100;
    public int count = 0;

    private Vector3 movement;
    private Rigidbody rb;
    private Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 25.0f;
        rb.drag = 0.5f;
        SetCountText();
        winText.text = "";
    }

    private void Update()
    {
        Debug.Log("Pickup Capture Time: " + captureTime);
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        movement = RotateWithView();

        rb.AddForce(movement * speed);
    }

    public void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 8)
        {
            winText.text = "You Win!";
        }
    }

    private Vector3 RotateWithView()
    {
        if (cameraTransform != null)
        {
            Vector3 direction = cameraTransform.TransformDirection(movement);
            direction.Set(direction.x, 0, direction.z);
            return direction.normalized * movement.magnitude;
        }

        else
        {
            cameraTransform = Camera.main.transform;
            return movement;
        }
    }
}
