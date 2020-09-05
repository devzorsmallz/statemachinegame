using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool movementDisabled = false;
    public float speed;
    public float dashSpeed;
    public int captureTime = 200;
    public int count = 0;
    public int score = 0;
    public int dashCooldown = 1000;
    public Text countText;
    public Text winText;
    public Text scoreText;
    public Text dashCDText;

    private bool dashHeld = false;
    private int minDashHold = 50;
    private Vector3 movement;
    private Rigidbody rb;
    private Transform cameraTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 10.0f;
        rb.drag = 0.5f;
        winText.text = "";
    }

    void Update()
    {
        Debug.Log("Pickup Capture Time: " + captureTime);
        Debug.Log("Score: " + score);
        scoreText.text = "Score: " + score.ToString();
        countText.text = "Count: " + count.ToString();

        dashCDText.text = "Dash Cooldown: " + dashCooldown.ToString();

        if (score == 8)
        {
            winText.text = "You Win!";
        }

        if (dashCooldown < 1000)
        {
            dashCooldown++;
        }

        if (dashHeld && minDashHold > 0)
        {
            minDashHold--;
            Debug.Log("Min Dash Hold: " + minDashHold);
        }

        if (dashCooldown == 1000 && Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) { }

            else
            {
                rb.velocity = Vector3.zero;
                rb.freezeRotation = true;
                rb.freezeRotation = false;
                dashHeld = true;
                movementDisabled = true;
            }
        }

        if (dashHeld && Input.GetKeyUp(KeyCode.Space))
        {
            if (minDashHold == 0)
            {
                if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) { }
                
                else
                {
                    rb.AddForce(movement * dashSpeed);
                    dashCooldown = 0;
                }
            }

            dashHeld = false;
            movementDisabled = false;
            minDashHold = 50;
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        movement = RotateWithView();

        if (!movementDisabled)
        {
            rb.AddForce(movement * speed);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            score += count;
            count = 0;
        }
    }
}
