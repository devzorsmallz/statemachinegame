using Packages.Rider.Editor.PostProcessors;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public bool hasCaptured = false;
    public int count = 0;
    public int score = 0;
    public Transform target;
    public float navigationUpdate;
    private float navigationTime = 0;
    private NavMeshAgent agent;
    public Vector3 lastPosition;
    public bool updateTarget;
    public bool TurnIn;
    public Transform nextTarget;
    public GameObject[] targetList;
    public GameObject player;
    public GameObject droppedCube;
    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        // Set the last position to zero
        lastPosition = Vector3.zero;
        // Set the target to the location of a GameObject with the name "Pick Up Holder"
        target = GameObject.Find("Pick Up Holder").transform;
    }

    void Update()
    {
        Debug.Log(count);

        // If the current target is destroyed, find another one
        if (target == null || nextTarget == null || !target.gameObject.activeInHierarchy || !nextTarget.gameObject.activeInHierarchy)
        {
            updateTarget = true;
        }

        // If the enemy has a cube, set TurnIn to true;
        if (hasCaptured && count != 0)
        {
            TurnIn = true;
        }

        // Find all GameObjects with the tag "Pick Up"
        targetList = GameObject.FindGameObjectsWithTag("Pick Up");

        // If the target has been updated, find a new random target based on the number of cubes that are still in play
        if (targetList.Length > 0 && updateTarget == true)
        {
            nextTarget = targetList[Random.Range(0,targetList.Length)].transform;

            // If the chosen next target exists, set it to the current target and stop updating the target
            if (nextTarget != null)
            {
                target = nextTarget;
                updateTarget = false;
            }
        }

        navigationTime += Time.deltaTime;
        if (navigationTime > navigationUpdate && target != null)
        {
            agent.destination = target.position;
            navigationTime = 0;
        }
        
        // If TurnIn is true, if the count goes to zero, switch back to finding cubes; otherwise, find the Goal
        if (TurnIn == true)
        {
            if (count == 0)
            {
                updateTarget = true;
            }

            else
            {
                target = GameObject.Find("Enemy Base Goal").transform;
            }
            
            TurnIn = false;
        }
    }

    private void FixedUpdate()
    {
        // If the current target does not exist, update the target
        if (target.gameObject.activeInHierarchy == false)
        {
            updateTarget = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // If the enemy touches the Goal, update the target
        if (col.tag == "EnemyGoal")
        {
            updateTarget = true;
            
            // If the enemy has cubes, exchange them for points
            if (count > 0)
            {
                score += count;
                player.GetComponent<PlayerController>().numCubes -= count;
                count = 0;
                hasCaptured = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player dashes into the enemy, the enemy drops its cubes
        if (collision.collider.tag == "Player" && collision.collider.GetComponent<PlayerController>().isDashing)
        {
            if (count > 0)
            {
                for (int i = 0; i < count;  i++)
                {
                    GameObject droppedCubeInstance;
                    droppedCubeInstance = Instantiate(droppedCube, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), transform.rotation) as GameObject;
                    droppedCubeInstance.GetComponent<Rigidbody>().AddForce(droppedCube.transform.up * 5.0f, ForceMode.Impulse);
                    TurnIn = false;
                    updateTarget = true;
                    Debug.Log("Hello");
                }

                count = 0;
            }
        }
    }
}
