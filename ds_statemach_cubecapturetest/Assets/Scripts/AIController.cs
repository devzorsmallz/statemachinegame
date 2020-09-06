using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Transform target;
    public float navigationUpdate;
    private float navigationTime = 0;
    private NavMeshAgent agent;
    public Vector3 lastPosition;
    public bool updateTarget;
    public bool TurnIn;
    public Transform nextTarget;
    public GameObject[] targetList;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = Vector3.zero;
        target= GameObject.Find("Pick Up Holder").transform;
    }

    // Update is called once per frame
    void Update()
    {
        targetList = GameObject.FindGameObjectsWithTag("Pick Up");
        if (targetList.Length > 0 && updateTarget == true)
        {
            nextTarget = targetList[Random.Range(0,8)].transform;

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
        if (col.tag == "Pick Up")
        {
            Destroy(col.gameObject);
            TurnIn = true;
        }
        if (col.tag == "EnemyGoal")
        {
            updateTarget = true;
        }
    }
}
