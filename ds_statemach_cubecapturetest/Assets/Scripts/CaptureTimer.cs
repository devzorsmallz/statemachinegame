using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureTimer : MonoBehaviour
{
    public bool playerHasCaptured = false;
    public GameObject player;

    private bool playerCapturing = false;
    private bool hasBeenCounted = false;
    private Animator anim;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().captureTime > 0 && playerCapturing)
        {
            if (player.GetComponent<PlayerController>().captureTime > 0)
            {
                player.GetComponent<PlayerController>().captureTime--;
                anim.SetBool("shrinkAnim", true);
            }
        }

        if (player.GetComponent<PlayerController>().captureTime == 0 && hasBeenCounted == false && playerCapturing)
        {
            playerHasCaptured = true;
            hasBeenCounted = true;
            player.GetComponent<PlayerController>().count++;
            player.GetComponent<PlayerController>().SetCountText();

            Debug.Log("Cube " + player.GetComponent<PlayerController>().count + " Captured!");
            player.GetComponent<PlayerController>().captureTime = 100;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerCapturing = false;
            anim.SetBool("shrinkAnim", false);
            player.GetComponent<PlayerController>().captureTime = 100;
        }
    }
}
