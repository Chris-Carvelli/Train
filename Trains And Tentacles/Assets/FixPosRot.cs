using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosRot : MonoBehaviour {
	public Transform target;
	float startingY;

	private void Start() {
		startingY = transform.position.y;
	}

	private void Update() {
		Vector3 newPos = target.position;
		newPos.y = startingY;
		transform.position = newPos;
	}
}
