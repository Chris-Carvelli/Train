using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RobotJoint))]
public class RobotArm : MonoBehaviour {
	public Transform target;

	//TMP make private
	public RobotJoint[] _joints;
	public float[] _angles;

	// Use this for initialization
	void Start () {
		_joints = GetComponentsInChildren<RobotJoint>();
		_angles = new float[_joints.Length];
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < _joints.Length; i++) {
			_joints[i].InverseKinematic(target.position, ref _angles);

			_joints[i].transform.localRotation = Quaternion.AngleAxis(_angles[i], _joints[i].axis);
		}
	}
}
