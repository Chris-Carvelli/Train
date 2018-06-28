using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO make this only a data container and move all the logic in RobotArm
public class RobotJoint : MonoBehaviour {
	//TODO move in RobotArm
	public static float LearningRate = 5f;
	public static float SamplingDistance = 1f;
	
	//data
	public Vector3 axis;
	public Vector3 startOffset;
	public float minAngle = 0.0f;
	public float maxAngle = 360;

	//service vars
	private RobotArm _arm;

	// Use this for initialization
	void Start () {
		startOffset = transform.localPosition;

		_arm = GetComponentInParent<RobotArm>();
	}

	public Vector3 ForwardKinematic(ref float[] angles) {
		Vector3 prevPoint = _arm._joints[0].transform.position;
		Quaternion rotation = Quaternion.identity;

		for (int i = 1; i < _arm._joints.Length; i++) {
			rotation *= Quaternion.AngleAxis(angles[i - 1], _arm._joints[i - 1].axis);
			Vector3 nextPoint = prevPoint + rotation * _arm._joints[i].startOffset;

			prevPoint = nextPoint;
		}

		return prevPoint;
	}

	public float DistanceFromTarget (Vector3 target, ref float[] angles) {
		Vector3 point = ForwardKinematic(ref angles);
		return Vector3.Distance(point, target);
	}

	public float PartialGradient (Vector3 target, ref float[] angles, int i) {
		float angle = angles[i];

		float f_x = DistanceFromTarget(target, ref angles);

		angles[i] += SamplingDistance;
		float f_x_plus_d = DistanceFromTarget(target, ref angles);

		float gradient = (f_x_plus_d - f_x) / SamplingDistance;

		angles[i] = angle;
			
		return gradient;
	}

	public void InverseKinematic (Vector3 target, ref float[] angles) {
		for (int i = 0; i < _arm._joints.Length; i++) {
			float gradient = PartialGradient(target, ref angles, i);
			angles[i] -= LearningRate * gradient;

			angles[i] = Mathf.Clamp(angles[i], minAngle, maxAngle);

		}
	}
}
