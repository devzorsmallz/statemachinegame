using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool movementDisabled = false;
    public float speed;
    public float dashSpeed;
    public int count = 0;
    public int score = 0;
    public int numCubes = 8;
    public int dashCooldown = 1000;
    public Text countText;
    public Text winText;
    public Text scoreText;
    public Text dashCDText;

    private bool dashHeld = false;
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
        if (numCubes == 0)
        {
            if (score < numCubes / 2)
            {
                winText.text = "You Lose!";
            }

            else if (score == numCubes / 2)
            {
                winText.text = "Tie!";
            }

            else
            {
                winText.text = "You Win!";
            }
        }
        //Debug.Log("Pickup Capture Time: " + captureTime);
        //Debug.Log("Score: " + score);
        scoreText.text = "Score: " + score.ToString();
        countText.text = "Count: " + count.ToString();

        dashCDText.text = "Dash Cooldown: " + dashCooldown.ToString();

        if (dashCooldown < 1000)
        {
            dashCooldown++;
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
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) { }
                
            else
            {
                rb.AddForce(movement * dashSpeed);
                dashCooldown = 0;
            }

            dashHeld = false;
            movementDisabled = false;
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
            if (count > 0)
            {
                score += count;
                numCubes-= count;
                count = 0;
            }
        }
    }
}
