using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CentralizedRail))]
public class ControlPointEditor : Editor {

	SerializedProperty _keys;
	SerializedProperty _vals;

	private void OnEnable() {
		if (target == null)
			return;

		_keys = serializedObject.FindProperty("_keys");
		_vals = serializedObject.FindProperty("_vals");
	}

	public void OnInspectorGUI(long id) {
		for (int i = 0; i < _keys.arraySize; i++)
			if (_keys.GetArrayElementAtIndex(i).longValue == id) {
				serializedObject.Update();

				EditorGUI.BeginChangeCheck();

				EditorGUILayout.PropertyField(_vals.GetArrayElementAtIndex(i));

				if (EditorGUI.EndChangeCheck())
					serializedObject.ApplyModifiedProperties();
			}
	}
}

