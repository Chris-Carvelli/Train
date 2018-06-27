using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrainController : MonoBehaviour {

    public NavMeshAgent agent;

    public GameObject target;

    public Camera cam;
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(target.transform.position);
	}
}
