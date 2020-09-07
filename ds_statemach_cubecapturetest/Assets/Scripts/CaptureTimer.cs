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

    // Update is called once per frame
    void Update()
    {
        if (!hasBeenCaptured && !enemyCapturing && playerCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

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

        if (!hasBeenCaptured && !playerCapturing && enemyCapturing)
        {
            StartCoroutine("CaptureCountdown", captureTime);
            anim.SetBool("shrinkAnim", true);
        }

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
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = true;
        }

        else if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCapturing = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }

        else if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCapturing = false;
            anim.SetBool("shrinkAnim", false);
            StopCoroutine("CaptureCountdown");
        }
    }

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
