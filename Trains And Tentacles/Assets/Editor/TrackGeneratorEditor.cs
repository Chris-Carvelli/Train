using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackGenerator))]
public class TrackGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Clean")) {
			(target as TrackGenerator).Clean();
		}

		if (GUILayout.Button("Generate")) {
			(target as TrackGenerator).Generate();
		}
	}
}
