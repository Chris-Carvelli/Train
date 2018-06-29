using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainController : MonoBehaviour {

    public Transform[] wayPoints;
	public Vector3 offsetFromTrack;

    private float speed;
    public float rotSpeed = 60f;

    public float maxSpeed = 50f;
    public float avgSpeed = 15f;
    public float minSpeed = 5f;

    public WaypointNode current;

    public bool speedControl;

    void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        speed = avgSpeed;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            while (speed < maxSpeed)
            {
                speed += 0.001f;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            while (speed > avgSpeed)
            {
                speed -= 0.001f;
            }
        }

        if (speedControl)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                while (speed > minSpeed)
                {
                    speed -= 0.001f;
                }
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                while (speed <= avgSpeed)
                {
                    speed += 0.001f;
                }
            }
        }
        
        if (transform.position != current.point.position)
        {
            Debug.Log(false);
            Vector3 pos = Vector3.MoveTowards(transform.position, current.point.position, speed * Time.deltaTime);
            Quaternion rot = Quaternion.RotateTowards(transform.rotation, current.point.rotation, rotSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos + offsetFromTrack);
            GetComponent<Rigidbody>().MoveRotation(rot);
        } else {
            Debug.Log(true);
            current = current.children[0];
            
        }
    }
}
