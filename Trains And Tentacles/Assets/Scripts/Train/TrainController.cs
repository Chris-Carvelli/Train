using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainController : MonoBehaviour {

    public Transform[] wayPoints;
	public Vector3 offsetFromTrack;

    public float speed = 10f;
    public float rotSpeed = 60f;

    public WaypointNode current;

    void Start()
    {
        GetComponentInChildren<Renderer>().material.color = Color.blue;
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

		//if (transform.position.x != current.point.position.x && transform.position.z != current.point.position.z)
		if (transform.position != current.point.position) {
            Debug.Log(false);
            Vector3 pos = Vector3.MoveTowards(transform.position, current.point.position, speed * Time.deltaTime);
            Quaternion rot = Quaternion.RotateTowards(transform.rotation, current.point.rotation, rotSpeed * Time.deltaTime);
			GetComponentInChildren<Rigidbody>().MovePosition(pos + offsetFromTrack);
			GetComponentInChildren<Rigidbody>().MoveRotation(rot);
        } else {
            Debug.Log(true);
            current = current.children[0];
            
        }
    }
}
