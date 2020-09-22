using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CaptureTimer : MonoBehaviour
{
    public bool hasBeenCaptured = false;
    public int captureTime;

    private bool playerCapturing = false;
    private bool enemyCapturing = false;
    private bool enemy1Capturing = false;
    private bool friendlyCapturing = false;
    private Animator anim;
    private GameObject player;
    private GameObject enemy;
    private GameObject enemy1;
    private GameObject friendly;
    public GameObject basePlatform;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        player = GameObject.Find("Player");
        enemy = GameObject.Find("Enemy");
        enemy1 = GameObject.Find("Enemy (1)");
        friendly = GameObject.Find("Friendly");

        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), true);
        Physics.IgnoreCollision(friendly.GetComponent<Collider>(), GetComponent<Collider>(), true);
        Physics.IgnoreCollision(enemy.GetComponent<Collider>(), GetComponent<Collider>(), true);
        if(enemy1!=null)Physics.IgnoreCollision(enemy1.GetComponent<Collider>(), GetComponent<Collider>(), true);
    }

    void Update()
    {
        // If the cube has not been captured, the enemy is not capturing it, and the player is capturing it, start the capture countdown and play the animation
        if (!hasBeenCaptured && !enemyCapturing && !enemy1Capturing && !friendlyCapturing && playerCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

        // If the cube has been captured by the player, increase the player's cube count, play a sound, and destroy the cube
        if (hasBeenCaptured && !enemyCapturing && !enemy1Capturing && !friendlyCapturing && playerCapturing)
        {
            player.GetComponent<PlayerController>().count++;

            Debug.Log("Player captured a cube! Player has " + player.GetComponent<PlayerController>().count + " cubes and " + player.GetComponent<PlayerController>().score + " points!");
            StopCoroutine("CaptureCountdown");

            player.GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => dac;
			repeat( 15 )
			{
				Math.random2f( 300, 700 ) => foo.freq;
				10::ms => now;
			}
		    ");

            this.gameObject.SetActive(false);
        }

        // If the cube has not been captured, the player is not capturing it, and the enemy is capturing it, start the capture countdown and play the animation
        if (!hasBeenCaptured && !playerCapturing && !friendlyCapturing && enemyCapturing || enemy1Capturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

        // If the cube has been captured by the enemy, increase the enemy's cube count, play a sound
        // Notify the enemy that it has captured the cube, and destroy the cube
        if (hasBeenCaptured && !playerCapturing && !friendlyCapturing && enemyCapturing || enemy1Capturing)
        {
            if (enemyCapturing) enemy.GetComponent<EnemyController>().count++;
            if (enemy1Capturing) enemy1.GetComponent<EnemyController>().count++;

            Debug.Log("Enemy captured a cube! Enemy has " + enemy.GetComponent<EnemyController>().count + " cubes and " + enemy.GetComponent<EnemyController>().score + " points!");
            StopCoroutine("CaptureCountdown");

            player.GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => dac;
			repeat( 15 )
			{
				Math.random2f( 700, 1000 ) => foo.freq;
				10::ms => now;
			}
		    ");

            if (enemyCapturing) enemy.GetComponent<EnemyController>().hasCaptured = true;
            if (enemy1Capturing) enemy1.GetComponent<EnemyController>().hasCaptured = true;
            hasBeenCaptured = false;
            if (enemyCapturing) enemyCapturing = false;
            if (enemy1Capturing) enemy1Capturing = false;
            this.gameObject.SetActive(false);
        }

        if (!hasBeenCaptured && !enemyCapturing && !enemy1Capturing && !playerCapturing && friendlyCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

        if (hasBeenCaptured && !enemyCapturing && !enemy1Capturing && !playerCapturing && friendlyCapturing)
        {
            friendly.GetComponent<FriendlyController>().count++;

            player.GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => dac;
			repeat( 15 )
			{
				Math.random2f( 300, 700 ) => foo.freq;
				10::ms => now;
			}
		    ");

            friendly.GetComponent<FriendlyController>().hasCaptured = true;
            hasBeenCaptured = false;
            friendlyCapturing = false;
            this.gameObject.SetActive(false);
        }
    }

    // When the cube hits the ground, it is now able to be picked up, and it won't fall through the floor
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<SphereCollider>().isTrigger = true;
            Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
            Physics.IgnoreCollision(friendly.GetComponent<Collider>(), GetComponent<Collider>(), false);
            Physics.IgnoreCollision(enemy.GetComponent<Collider>(), GetComponent<Collider>(), false);
            if (enemy1 != null) Physics.IgnoreCollision(enemy1.GetComponent<Collider>(), GetComponent<Collider>(), false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the cube is in contact with the player, set playerCapturing to true
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = true;
        }

        // If the cube is in contact with the enemy, set enemyCapturing to true
        else if (other.gameObject.CompareTag("Enemy") && other.gameObject.name == "Enemy")
        {
            enemyCapturing = true;
        }
        else if (other.gameObject.CompareTag("Enemy") && other.gameObject.name == "Enemy (1)")
        {
            enemy1Capturing = true;
        }

        else if (other.gameObject.CompareTag("Friendly"))
        {
            friendlyCapturing = true;
        }

        // If the cube falls through the floor, it will respawn above the arena
        else if (other.gameObject.CompareTag("Death Area"))
        {
            transform.position = new Vector3(basePlatform.transform.position.x, basePlatform.transform.position.y + 3.0f, basePlatform.transform.position.z);
            GetComponent<SphereCollider>().isTrigger = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // If the player leaves the cube, set playerCapturing to false, stop playing the animation, and stop the countdown
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }

        // If the enemy leaves the cube, set enemyCapturing to false, stop playing the animation, and stop the countdown
        else if (other.gameObject.CompareTag("Enemy") && other.gameObject.name == "Enemy")
        {
            enemyCapturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }
        else if (other.gameObject.CompareTag("Enemy") && other.gameObject.name == "Enemy (1)")
        {
            enemy1Capturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }

        else if (other.gameObject.CompareTag("Friendly"))
        {
            friendlyCapturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }
    }

    // Capture countdown timer
    private IEnumerator CaptureCountdown(int time)
    {
        while (time > 0)
        {
            time--;
            yield return new WaitForSeconds(0.25f);
        }

        if (time == 0)
        {
            hasBeenCaptured = true;
        }
    }
}
