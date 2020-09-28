using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    public static int state = 1;
    public bool hasCaptured = false;
    public int count = 0;
    public int score = 0;
    public int dazedTime = 2;
    public Transform target;
    public float navigationUpdate;
    private float navigationTime = 0;
    private NavMeshAgent agent;
    public bool targetIsUpdated = false;
    public bool TurnIn;
    public Transform nextTarget;
    public GameObject[] targetList;
    public GameObject droppedCube;
    public float distanceToPlayer;
    private Rigidbody rb;
    public bool dashed = false;
    public bool dazed = false;
    public GameObject dazedEffect;
    public Vector3 initialPosition;

    private GameObject dazedEffectInstance;

    public static bool gotHit = false;
    public bool dazePlayer=false;

    void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        targetList = GameObject.FindGameObjectsWithTag("Pick Up");
        target = targetList[Random.Range(0, targetList.Length)].transform;
        dazedEffectInstance = Instantiate(dazedEffect);
        dazedEffectInstance.SetActive(false);
    }
    void Update()
    {
        if(gotHit)
        {
            StartCoroutine("hitDelay");
        }
        if (dazedEffectInstance)
        {
            dazedEffectInstance.transform.position = transform.position;
        }

        //tells the agent to move towards the target
        navigationTime += Time.deltaTime;
        if (navigationTime > navigationUpdate && target != null && agent.isOnNavMesh)
        {
            agent.destination = target.position;
            navigationTime = 0;
        }

        //switch to return state if captures a pickup
        if (hasCaptured && count != 0)
        {
            state = 2;
            StateChange();
        }
        //check the distance between player and enemy if dash isnt on cooldown
        if (dashed == false)
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        }

        //switch to attack state if player gets close and isnt dashing
        if (distanceToPlayer < 1.5F && player.GetComponent<PlayerController>().isDashing == false && !dazed && !gotHit)
        {
            dashed = true;
            distanceToPlayer = 5;
            state = 3;
            StateChange();
        }
        if(distanceToPlayer<2F && player.GetComponent<PlayerController>().isDashing)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        } 

        if (dazed)
        {
            GetComponent<NavMeshAgent>().speed = 0;
        }

        else
        {
            GetComponent<NavMeshAgent>().speed = 10;
        }
    }
    private void FixedUpdate()
    {
        // If the current target does not exist, update the target
        if (target.gameObject.activeInHierarchy == false)
        {
            UpdateTarget();
        }
    }

    public void StateChange()
    {
        switch (state)
        {
            case 3:
                //Attack
                AttackPlayer();
                print("Attacking");
                break;

            case 2:
                //Return
                ReturnToBase();
                print("Returning Points");
                break;

            case 1:
                //Retrieve
                GetPoints();
                print("Retrieving Points");
                break;

            default:
                print("Invalid phase");
                break;
        }
    }
    public void GetPoints()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        agent.isStopped = false;
        //get a new target if one is missing
        if (target == null || target.gameObject.activeInHierarchy == false)
        { targetIsUpdated = false; }
        //get new target when state changes
        if (targetIsUpdated == false)
        {
            rb.mass = 10;
            agent.speed = 8;
            agent.acceleration = 20;
            targetIsUpdated = true;
            UpdateTarget();
        }
    }
    public void AttackPlayer()
    {
        dazePlayer = true;
        rb.mass = 200;
        rb.AddForce((player.transform.position - transform.position) * 99999);
        if (dashed == true)
        {
            StartCoroutine("myDelay");
        }
    }
    public void ReturnToBase()
    {
        GetComponent<NavMeshAgent>().enabled = true;
        agent.isStopped = false;
        agent.speed = 8;
        agent.acceleration = 20;
        rb.mass = 10;
        target = GameObject.Find("Enemy Base Goal").transform;
    }

    public void UpdateTarget()
    {
        //search for available pickups and target a random one
        targetList = GameObject.FindGameObjectsWithTag("Pick Up");
        if (targetList.Length > 0)
        {
            nextTarget = targetList[Random.Range(0, targetList.Length)].transform;
        }
        // If the chosen next target exists, set it to the current target and stop updating the target
        if (nextTarget != null)
        {
            target = nextTarget;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        // If the enemy touches the Goal, update the target
        if (col.tag == "EnemyGoal")
        {
            // If the enemy has cubes, exchange them for points
            score += count;
            player.GetComponent<PlayerController>().numCubes -= count;
            count = 0;
            hasCaptured = false;
            targetIsUpdated = false;
            state = 1;
            StateChange();
        }
        if (col.CompareTag("Death Area"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = initialPosition;
            GetComponent<NavMeshAgent>().enabled = true;
            count = 0;
            hasCaptured = false;
            targetIsUpdated = false;
            state = 1;
            StateChange();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player dashes into the enemy, the enemy drops its cubes and then locates a new target
        if (collision.collider.tag == "Player" && gotHit)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            hasCaptured = false;
            dazed = true;
            StartCoroutine("DazedCountdown", dazedTime);
            StartCoroutine("stopAfterDelay");
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    GameObject droppedCubeInstance;
                    droppedCubeInstance = Instantiate(droppedCube, new Vector3(transform.position.x, transform.position.y + 3.0f, transform.position.z), transform.rotation) as GameObject;
                    droppedCubeInstance.GetComponent<Rigidbody>().AddForce(droppedCube.transform.up * 5.0f, ForceMode.Impulse);
                    //reset



                    print("Enemy Drops Point");
                }
                count = 0;
                hasCaptured = false;
            }

        }
        //reset the enemy upon collision with player during attack state
        if (collision.collider.tag == "Player" && dazePlayer)
        {
            player.GetComponent<PlayerController>().StartCoroutine("DazedCountdown", player.GetComponent<PlayerController>().dazedTime);
            //stop and reset
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            agent.isStopped = true;
            agent.isStopped = false;
            dazePlayer = false;
        }
    }

    private IEnumerator myDelay()
    {
        //delays the distance check to prevent the enemy from repeatedly dashing
        yield return new WaitForSeconds(5f);
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        dashed = false;
    }
    private IEnumerator hitDelay()
    {
        //delays the distance check to prevent the enemy from repeatedly dashing
        yield return new WaitForSeconds(1f);
        gotHit = false;
    }
    private IEnumerator stopAfterDelay()
    {
        //stops rigidbody movement after a delay
        yield return new WaitForSeconds(2f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        GetComponent<NavMeshAgent>().enabled = true;
        agent.isStopped = false;
        targetIsUpdated = false;
        state = 1;
        StateChange();
    }

    private IEnumerator DazedCountdown(int time)
    {
        dazedEffectInstance.SetActive(true);

        while (time > 0)
        {
            time -= 2;
            yield return new WaitForSeconds(2.0f);
        }

        if (time == 0)
        {
            dazed = false;
            dazedEffectInstance.SetActive(false);
        }
    }
}
