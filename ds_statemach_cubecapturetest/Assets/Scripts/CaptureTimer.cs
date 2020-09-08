using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CaptureTimer : MonoBehaviour
{
    public bool hasBeenCaptured = false;
    public int captureTime;
    public GameObject player;
    public GameObject enemy;

    private bool playerCapturing = false;
    private bool enemyCapturing = false;
    private Animator anim;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // If the cube has not been captured, the enemy is not capturing it, and the player is capturing it, start the capture countdown and play the animation
        if (!hasBeenCaptured && !enemyCapturing && playerCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

        // If the cube has been captured by the player, increase the player's cube count, play a sound, and destroy the cube
        if (hasBeenCaptured && !enemyCapturing && playerCapturing)
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
        if (!hasBeenCaptured && !playerCapturing && enemyCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

        // If the cube has been captured by the enemy, increase the enemy's cube count, play a sound, notify the enemy that it has captured the cube, and destroy the cube
        if (hasBeenCaptured && !playerCapturing && enemyCapturing)
        {
            enemy.GetComponent<AIController>().count++;

            Debug.Log("Enemy captured a cube! Enemy has " + enemy.GetComponent<AIController>().count + " cubes and " + enemy.GetComponent<AIController>().score + " points!");
            StopCoroutine("CaptureCountdown");

            player.GetComponent<ChuckSubInstance>().RunCode(@"
			SinOsc foo => dac;
			repeat( 15 )
			{
				Math.random2f( 700, 1000 ) => foo.freq;
				10::ms => now;
			}
		    ");

            enemy.GetComponent<AIController>().hasCaptured = true;
            this.gameObject.SetActive(false);
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
        else if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCapturing = true;
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
        else if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCapturing = false;
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
            yield return new WaitForSeconds(1);
        }

        if (time == 0)
        {
            hasBeenCaptured = true;
        }
    }
}
