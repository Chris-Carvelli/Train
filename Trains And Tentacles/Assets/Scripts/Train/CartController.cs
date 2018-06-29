using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartController : MonoBehaviour {
	[Header("Runtime Info")]
	public CartController prevCart;
	public WaypointNode prev;
	public WaypointNode curr;
	public WaypointNode next;
	public TrainController trainController;
	public Vector3 meshDimensions;

	// Use this for initialization
	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		meshDimensions = mesh.bounds.size;

		float distance = Vector3.Distance(transform.localPosition, next.transform.localPosition);
		float offset = (meshDimensions.z * 2F) / distance;
		t = prevCart == null ? 0.5f : prevCart.t - offset;
	}
	
	// Update is called once per frame
	void Update () {
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

	private bool Switch() {
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
		t += (GetSpeed() * Time.deltaTime) / distance;

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
		t += (GetSpeed() * Time.deltaTime) / distance;

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

	private Vector3 GetDirection(Vector3 a, Vector3 b) {
		return (b - a).normalized;
	}

	private Vector3 GetDirection(Transform a, Transform b) {
		return GetDirection(a.localPosition, b.localPosition);
	}

	public float GetSpeed() {
		return TrainController.speed;
	}

	public void Forward () {
		prev = curr ?? prevCart.prev;
		curr = next ?? prevCart.curr;
		next = curr.children[curr.nextChild];
	}
}
