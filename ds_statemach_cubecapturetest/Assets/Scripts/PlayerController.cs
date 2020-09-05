using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public Text scoreText;
    public int captureTime = 200;
    public int count = 0;
    public int score = 0;

    private Vector3 movement;
    private Rigidbody rb;
    private Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 25.0f;
        rb.drag = 0.5f;
        winText.text = "";
    }

    void Update()
    {
        Debug.Log("Pickup Capture Time: " + captureTime);
        Debug.Log("Score: " + score);
        scoreText.text = "Score: " + score.ToString();
        countText.text = "Count: " + count.ToString();

        if (score == 8)
        {
            winText.text = "You Win!";
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        movement = RotateWithView();

        rb.AddForce(movement * speed);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            score += count;
            count = 0;
        }
    }
}
