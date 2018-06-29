using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrainController : MonoBehaviour {
	[Header("Runtime Info")]
	public int nextChildIndex = 0;
	public WaypointNode prev;
	public WaypointNode curr;
	public WaypointNode next;

	public Transform[] wayPoints;

    public float speed = 10f;
    public float rotSpeed = 60f;

	[Header("SpeedManagement")]
	public float maxSpeed = 50f;
	public float avgSpeed = 15f;
	public float minSpeed = 5f;

	public bool speedControl;


	void Start()
    {
		//TODO auto get prev and next
        GetComponentInChildren<Renderer>().material.color = Color.blue;

		speed = avgSpeed;
    }

	// Update is called once per frame
	void Update() {
		ProcessControls();

		if (turning)
			DoTurn();
		else
			DoStraight();

		if (Switch()) {
			if (turning) {
				Forward();
			}
			turning = !turning;
			t -= 1;
		}
	}

	[Header("Debug")]
	public bool turning = false;
	private bool IsTurning() {
		float d = Vector3.Distance(transform.localPosition, curr.transform.localPosition);
		return d <= curr.radius;
	}

	private bool Switch () {
		//return IsTurning() ^ turning;
		return t >= 1;
		//float alpha = Vector3.Dot(transform.forward, curr.children[nextChildIndex].transform.localPosition - curr.transform.localPosition);
		//bool a = !Turn();
		//bool b = alpha > 0.9;
		//return a && b;
	}

	public float t = 0;
	public Vector3 positionA;
	public Vector3 positionB;

	//TODO unite DoStraight and DoTurn, just get different posA and posB
	private void DoStraight() {
		//Vector3 pos = Vector3.MoveTowards(transform.position, current.transform.position, speed * Time.deltaTime);
		//Quaternion rot = Quaternion.RotateTowards(transform.rotation, current.transform.rotation, rotSpeed * Time.deltaTime);
		//GetComponentInChildren<Rigidbody>().MovePosition(pos);
		//GetComponentInChildren<Rigidbody>().MoveRotation(rot);

		Vector3 posA = prev.transform.localPosition + GetDirection(prev.transform, curr.transform) * prev.radius;
		Vector3 posB = curr.transform.localPosition + GetDirection(curr.transform, prev.transform) * curr.radius;

		float distance = Vector3.Distance(posA, posB);
		t += (speed * Time.deltaTime) / distance;

		Vector3 pos = Vector3.Lerp(posA, posB, t);
		transform.localPosition = pos;
		
		//DEBUG
		positionA = posA;
		positionB = posB;
	}

	private void DoTurn() {
		//Vector3 pos = Vector3.MoveTowards(transform.position, current.children[nextChildIndex].transform.position, speed * Time.deltaTime);
		//Quaternion rot = Quaternion.RotateTowards(transform.rotation, current.children[nextChildIndex].transform.rotation, rotSpeed * Time.deltaTime);
		//GetComponentInChildren<Rigidbody>().MovePosition(pos);
		//GetComponentInChildren<Rigidbody>().MoveRotation(rot);

		Vector3 dirA = GetDirection(curr.transform, prev.transform);
		Vector3 dirB = GetDirection(curr.transform, next.transform);

		Vector3 posA = curr.transform.localPosition + dirA * curr.radius;
		Vector3 posB = curr.transform.localPosition + dirB * curr.radius;

		Quaternion rotA = Quaternion.LookRotation(GetDirection(prev.transform, curr.transform), Vector3.up);
		Quaternion rotB = Quaternion.LookRotation(GetDirection(curr.transform, next.transform), Vector3.up);

		float distance = Vector3.Distance(posA, posB);
		t += (speed * Time.deltaTime) / distance;

		Vector3 x = Vector3.Lerp(posA, curr.transform.position, t);
		Vector3 y = Vector3.Lerp(curr.transform.position, posB, t);

		Vector3 pos = Vector3.Lerp(x, y, t);
		//Vector3 pos = Vector3.Slerp(posA, posB, t);
		Quaternion rot = Quaternion.Slerp(rotA, rotB, t);

		transform.localPosition = pos;
		transform.localRotation = rot;
		
		//DEBUG
		positionA = posA;
		positionB = posB;
	}

	private Vector3 GetDirection (Vector3 a, Vector3 b) {
		return (b - a).normalized;
	}

	private Vector3 GetDirection(Transform a, Transform b) {
		return GetDirection (a.localPosition, b.localPosition);
	}

	private void Forward () {
		nextChildIndex = 0;
		prev = curr;
		curr = next;
		next = curr.children[nextChildIndex];
	}

	private void DoSwitch () {
		nextChildIndex = ++nextChildIndex % curr.children.Length;
		next = curr.children[nextChildIndex];
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Train Hit");
    }

    private void ProcessControls() {
		if (Input.GetKeyDown("space"))
			DoSwitch();

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
}
