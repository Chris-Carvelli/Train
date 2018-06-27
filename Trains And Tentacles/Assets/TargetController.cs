using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

    public Transform[] wayPoints;

    public float speed = 10f;

    private int current;

    // Update is called once per frame
    void Update () {
        if(transform.position != wayPoints[current].position)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, wayPoints[current].position, speed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(pos);
        } else {
            current = (current + 1) % wayPoints.Length;
        }
    }
}
