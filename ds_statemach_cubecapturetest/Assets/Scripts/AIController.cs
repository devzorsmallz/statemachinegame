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
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = Vector3.zero;
        target= GameObject.Find("Pick Up Holder").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasCaptured)
        {
            TurnIn = true;
        }

        targetList = GameObject.FindGameObjectsWithTag("Pick Up");
        if (targetList.Length > 0 && updateTarget == true)
        {
            nextTarget = targetList[Random.Range(0,player.GetComponent<PlayerController>().numCubes - 1)].transform;

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
        if (TurnIn == true)
        {
            target = GameObject.Find("Enemy Base Goal").transform;
            TurnIn = false;
        }
    }
    private void FixedUpdate()
    {
        if (target.gameObject.activeInHierarchy == false)
        {
            updateTarget = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "EnemyGoal")
        {
            updateTarget = true;
            if (count > 0)
            {
                score += count;
                player.GetComponent<PlayerController>().numCubes -= count;
                count = 0;
                hasCaptured = false;
            }
        }
    }
}
