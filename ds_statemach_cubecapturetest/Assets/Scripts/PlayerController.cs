using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public bool movementDisabled = false;
    public bool isDashing = false;
    public bool canDash = true;
    public bool dazed = false;
    public float speed;
    public float dashSpeed;
    public int count = 0;
    public int score = 0;
    public int enemyScore = 0;
    public int enemyScore1 = 0;
    public int enemyScores = 0;
    public int initialNumCubes = 8;
    public int numCubes;
    public int dashCooldown = 5;
    public int currentDashCooldown;
    public int dazedTime = 2;
    public Text countText;
    public Text winText;
    public Text scoreText;
    public Text enemyScoreText;
    public Text dashCooldownText;
    public GameObject cam;
    public GameObject droppedCube;
    public GameObject dazedEffect;

    private bool dashHeld = false;
    private Vector3 movement;
    private Rigidbody rb;
    private Transform cameraTransform;
    private GameObject dazedEffectInstance;
    public GameObject enemy;
    public GameObject enemy1;


    void Start()
    {
        currentDashCooldown = 0;
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 10.0f;
        rb.drag = 0.5f;
        winText.text = "";
        numCubes = initialNumCubes;
        dazedEffectInstance = Instantiate(dazedEffect);
        dazedEffectInstance.SetActive(false);
    }

    void Update()
    {
        if (dazedEffectInstance)
        {
            dazedEffectInstance.transform.position = transform.position;
        }

        // Set the Dash Cooldown text to reflect the current dash cooldown
        dashCooldownText.text = "Dash Cooldown: " + currentDashCooldown;

        if (isDashing)
        {
            // While the player is dashing, their mass is increased, so when they hit an enemy, it goes flying
            rb.mass = 100;
            // If you miss the enemy, you are "confused" for 2 seconds, meaning you cannot move. This also prevents adjusting trajectory during dash.
            StartCoroutine("DashCoroutine", 2);
        }

        // If you are not dashing and the spacebar is not held down, your mass returns to 1 and your movement is re-enabled
        else if (!isDashing && !dashHeld && !dazed)
        {
            rb.mass = 1;
            movementDisabled = false;
        }
        enemyScore = enemy.GetComponent<EnemyController>().score;
        if (enemy1 != null)
        {
            enemyScore1 = enemy1.GetComponent<EnemyController>().score;
        }
        enemyScores = enemyScore + enemyScore1;
        // Win Text
        if (numCubes == 0)
        {
            // If you have fewer than half of the cubes when they have all been brought to a goal, you lose
            if (score < enemyScores)
            {
                winText.text = "You Lose!";
            }

            // If you have exactly half, you tie
            else if (score == enemyScores)
            {
                winText.text = "Tie!";
            }
            
            // If you have more than half, you win
            else
            {
                winText.text = "You Win!";
            }
        }

        // Set score text and count text
        scoreText.text = "Score: " + score.ToString();
        countText.text = "Count: " + count.ToString();
        enemyScoreText.text = "Enemy Score: " + enemyScores.ToString();

        // If the dash cooldown is zero, you are not currently dashing, and you hold down both space and forward, you will not be able to move until you let go of space
        // The camera zooms in slightly to indicate that you are about to perform a dash; camera zoom control is taken from the player
        if (canDash && !isDashing && Input.GetKeyDown(KeyCode.Space) && Input.GetAxis("Vertical") > 0)
        {
            dashHeld = true;
            movementDisabled = true;
            cam.GetComponent<CameraZoom>().canZoom = false;
            cam.transform.position += cam.transform.forward * 3;
        }

        if (dashHeld && Input.GetKeyUp(KeyCode.Space))
        {
            // If you simply let go of space without holding forward, the dash is canceled
            if (Input.GetAxis("Vertical") == 0) { }
            
            // If you let go of space and are still holding forward, you get launched in the direction the camera is facing, and your dash goes on cooldown
            else
            {
                rb.AddForce(new Vector3(cam.transform.forward.x, 0.0f, cam.transform.forward.z) * dashSpeed);
                isDashing = true;
                canDash = false;
                StartCoroutine("DashCooldownCoroutine", dashCooldown);
            }

            // The camera zooms back out, and camera zoom control is returned to the player
            dashHeld = false;
            cam.GetComponent<CameraZoom>().canZoom = true;
            cam.transform.position -= cam.transform.forward * 3;
        }
    }

    void FixedUpdate()
    {
        // Set horizontal and vertical inputs to their respective axes
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        
        // Set movement to the horizontal and vertical inputs
        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Set movement to the value returned by RotateWithView
        movement = RotateWithView();

        // If movement is not disabled, move according to inputs
        if (!movementDisabled)
        {
            rb.AddForce(movement * speed);
        }
    }

    // Rotate input with view (i.e. forward direction changes depending on which way you are facing)
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

    // When you touch the Goal, if you have cubes, exchange them for points
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

    // When you collide with an enemy while you are dashing, stop moving and re-enable movement
    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.collider.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            StopCoroutine("DashCoroutine");
            isDashing = false;
        }

        if (collision.collider.tag == "Enemy" && collision.collider.GetComponent<EnemyController>().dashed && !collision.collider.GetComponent<EnemyController>().dazed)
        {
            Debug.Log("Pog");

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject droppedCubeInstance;
                    droppedCubeInstance = Instantiate(droppedCube, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), transform.rotation) as GameObject;
                    droppedCubeInstance.GetComponent<Rigidbody>().AddForce(droppedCube.transform.up * 5.0f, ForceMode.Impulse);
                }

                count = 0;
            }
        }
    }

    // If you miss the enemy while dashing, you cannot move for two seconds
    private IEnumerator DashCoroutine(int time)
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(1);
        }

        if (time == 0)
        {
            isDashing = false;
        }
    }

    // Dash cooldown timer
    private IEnumerator DashCooldownCoroutine(int time)
    {
        while (time > 0)
        {
            currentDashCooldown = time;
            time--;
            yield return new WaitForSeconds(1);
        }

        if (time == 0)
        {
            currentDashCooldown = time;
            canDash = true;
        }
    }

    private IEnumerator DazedCountdown(int time)
    {
        dazedEffectInstance.SetActive(true);

        while (time > 0)
        {
            dazed = true;
            movementDisabled = true;
            canDash = false;
            time--;
            yield return new WaitForSeconds(1.0f);
        }

        if (time == 0)
        {
            dazed = false;
            movementDisabled = false;
            canDash = true;
            dazedEffectInstance.SetActive(false);
        }
    }
}
