using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartController : MonoBehaviour {
	[Header("Runtime Info")]
	public CartController prevCart;
	public ControlPoint prev;
	public ControlPoint curr;
	public ControlPoint next;
	public TrainController trainController;
	public Vector3 meshDimensions;

	// Use this for initialization
	void Start () {
		Mesh mesh = GetComponentInChildren<MeshFilter>().sharedMesh;
		meshDimensions = mesh.bounds.size;

		float distance = Vector3.Distance(transform.localPosition, next.position);
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
			t = 0;
		}
	}

	[Header("Debug")]
	public bool turning = false;
	private bool IsTurning() {
		float d = Vector3.Distance(transform.localPosition, curr.position);
		return d <= curr.scale.x;
	}

	private bool Switch() {
		return t >= 1;
	}

	public float t = 0;
	public Vector3 positionA;
	public Vector3 positionB;

	//TODO unite DoStraight and DoTurn, just get different posA and posB
	private void DoStraight() {

		Vector3 posA = prev.position + GetDirection(prev.position, curr.position) * prev.scale.x;
		Vector3 posB = curr.position + GetDirection(curr.position, prev.position) * curr.scale.x;

		float distance = Vector3.Distance(posA, posB);
		t += (GetSpeed() * Time.deltaTime) / distance;

		Vector3 pos = Vector3.Lerp(posA, posB, t);
		transform.localPosition = pos;

		//DEBUG
		positionA = posA;
		positionB = posB;
	}

	private void DoTurn() {
		Vector3 dirA = GetDirection(curr.position, prev.position);
		Vector3 dirB = GetDirection(curr.position, next.position);

		Vector3 posA = curr.position + dirA * curr.scale.x;
		Vector3 posB = curr.position + dirB * curr.scale.x;

		Quaternion rotA = Quaternion.LookRotation(GetDirection(prev.position, curr.position), Vector3.up);
		Quaternion rotB = Quaternion.LookRotation(GetDirection(curr.position, next.position), Vector3.up);

		float distance = Vector3.Distance(posA, posB);
		//float distance = (Mathf.PI / 2) * curr.radius;
		t += (GetSpeed() * Time.deltaTime) / distance;

		Vector3 x = Vector3.Lerp(posA, curr.position, t);
		Vector3 y = Vector3.Lerp(curr.position, posB, t);

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
		return a.GetDirection(b);
	}

	public float GetSpeed() {
		return TrainController.speed;
	}

	public void Forward () {
		//prev = curr ?? prevCart.prev;
		//curr = next ?? prevCart.curr;
		//next = curr.NextChild(prev);
	}
}
