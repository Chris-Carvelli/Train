using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainController : MonoBehaviour {
	[Header("Runtime Info")]
	public Transform[] wayPoints;

	[Header("Config")]
	public CartController head;
	public static float speed = 10f;
    public float rotSpeed = 60f;

	public float maxSpeed = 50f;
	public float avgSpeed = 15f;
	public float minSpeed = 5f;

	public bool speedControl;

	private TrainManager manager;

	void Start()
    {
		speed = avgSpeed;

		manager = GetComponent<TrainManager>();
    }

	// Update is called once per frame
	void Update() {
		ProcessControls();
	}

	private void DoSwitch () {
		if (!head.turning)
			head.next.DoSwitch(head.curr);
	}

	private void ProcessControls() {

		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			while (speed < maxSpeed) {
				speed += 0.001f;
			}
		}

		if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
			while (speed > avgSpeed) {
				speed -= 0.001f;
			}
		}

		if (speedControl) {
			if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
				while (speed > minSpeed) {
					speed -= 0.001f;
				}
			}

			if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
				while (speed <= avgSpeed) {
					speed += 0.001f;
				}
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log("Train Hit");
	}
}
