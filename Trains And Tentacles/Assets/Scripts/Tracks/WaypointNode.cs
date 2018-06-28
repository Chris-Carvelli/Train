using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour {

    public Transform point;
    public WaypointNode[] children;

    // Use this for initialization
    void Start () {
        point = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
