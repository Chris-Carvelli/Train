using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//TODO make proper layout
[CustomPropertyDrawer(typeof(IProcGenElementProperty), true)]
public class IProcGenElementPropertyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect cursor = position;

		IProcGenElement target = property.serializedObject.targetObject as IProcGenElement;

		cursor.width /= 2;

		if (GUI.Button(cursor, "Clean"))
			target.Clean();

		cursor.x += cursor.width;
		if (GUI.Button(cursor, "Generate"))
			target.Generate();
	}
}
