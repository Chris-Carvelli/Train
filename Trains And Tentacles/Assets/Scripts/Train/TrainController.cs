using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainController : MonoBehaviour {

    public Transform[] wayPoints;

    public float speed = 10f;
    private float rotSpeed = 60f;

    public WaypointNode current;

    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
        {
            if (current.children.Length > 1)
            {
                current = current.children[1];
            }
        }
        
        if (transform.position != current.point.position)
        {
            Debug.Log(false);
            Vector3 pos = Vector3.MoveTowards(transform.position, current.point.position, speed * Time.deltaTime);
            //Quaternion rot = Quaternion.RotateTowards(transform.rotation, current.point.rotation, rotSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
            GetComponent<Rigidbody>().MoveRotation(current.point.rotation);
        } else {
            Debug.Log(true);
            current = current.children[0];
            
        }
    }
}
