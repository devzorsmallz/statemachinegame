using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    private Vector3 randomHover;

    // Start is called before the first frame update
    void Start()
    {
        randomHover = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        while (true)
        {
            transform.position += randomHover;
            randomHover = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.5f, 0.5f));
            new WaitForSeconds(1);
        }
    }
}
