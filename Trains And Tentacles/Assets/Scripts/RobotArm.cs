using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RobotJoint))]
public class RobotArm : MonoBehaviour {
	public Transform target;

	//TMP make private
	private RobotJoint[] _joints;
	public float[] _angles;

	// Use this for initialization
	void Start () {
		_joints = GetComponentsInChildren<RobotJoint>();
		_angles = new float[_joints.Length];
	}
	
	// Update is called once per frame
	void Update () {
		foreach (RobotJoint joint in _joints)
			joint.InverseKinematic(target.position, ref _angles);
	}
}
