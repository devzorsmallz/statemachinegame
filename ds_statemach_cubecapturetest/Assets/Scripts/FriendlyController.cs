using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyController : MonoBehaviour
{
    public bool hasCaptured = false;
    public int count;
    public int score;

    private bool targetSelected = false;
    private float closestDistance;
    private NavMeshAgent agent;
    private GameObject player;
    private GameObject target;
    private GameObject closestTarget;
    private GameObject goal;
    private GameObject[] enemies;
    private GameObject[] targets;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        goal = GameObject.Find("Base Goal");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("Pick Up");

        if (!hasCaptured && targets.Length > 0)
        {
            closestDistance = Vector3.Distance(targets[0].transform.position, transform.position);
        }

        foreach (GameObject t in targets)
        {
            float distance = Vector3.Distance(t.transform.position, transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }

            if (Vector3.Distance(t.transform.position, transform.position) == closestDistance)
            {
                closestTarget = t;
            }
        }

        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < 5.0f)
            {
                Vector3 directionToEnemy = transform.position - enemy.transform.position;
                agent.destination = transform.position + directionToEnemy;
            }

            else
            {
                if (targetSelected)
                {
                    agent.destination = target.transform.position;
                }
            }
        }

        //Debug.Log("Friendly AI Count: " + count);

        score = player.GetComponent<PlayerController>().score;

        if (!targetSelected && count == 0 && player.GetComponent<PlayerController>().numCubes > 0)
        {
            target = closestTarget;
            agent.destination = target.transform.position;
            targetSelected = true;
        }

        if (target == null || !target.activeInHierarchy)
        {
            targetSelected = false;
        }

        if (hasCaptured && count > 0)
        {
            agent.destination = goal.transform.position;
        }
    }
    private void LateUpdate()
    {
        // If the current target does not exist, update the target
        if (target.gameObject.activeInHierarchy == false)
        {
            targetSelected = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasCaptured && count > 0 && other.gameObject.CompareTag("Goal"))
        {
            player.GetComponent<PlayerController>().numCubes -= count; ;
            player.GetComponent<PlayerController>().score += count;
            count = 0;
            hasCaptured = false;
            targetSelected = false;
        }
    }
}
