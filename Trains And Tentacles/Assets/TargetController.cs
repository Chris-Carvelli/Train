using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetController : MonoBehaviour {

    public Transform[] wayPoints;

    public float speed = 10f;

    public WaypointNode current;

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
        {
            if (current.children.Length > 1)
            {
                current = current.children[1];
            }
        }
        Debug.Log(current.point);

        if (transform.position != current.point.position)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, current.point.position, speed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
        } else {
            current = current.children[0];
            
        }
    }
}
