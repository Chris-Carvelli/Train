using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralRobotArm))]
public class ProceduralRobotArmEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Generate")) {
			(target as ProceduralRobotArm).Generate();
		}
	}
}
