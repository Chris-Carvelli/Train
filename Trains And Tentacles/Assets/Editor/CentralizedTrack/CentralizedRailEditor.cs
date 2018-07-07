using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CentralizedRail))]
public class CentralizedRailEditor : Editor {
	private SerializedProperty trackList;
	private SerializedProperty switchList;

	private void OnEnable() {
		trackList = serializedObject.FindProperty("tracks");
		switchList = serializedObject.FindProperty("switches");
	}

	private void OnSceneGUI() {
		
	}
}
