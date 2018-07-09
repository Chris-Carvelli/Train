using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorGUIUtils;

[CustomPropertyDrawer(typeof(ControlPointRef))]
public class ControlPointRefPropertyDrawer : PropertyDrawer {
	private float _increment = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect totalPosition = position;
		Rect cursor = position;

		SerializedProperty railProp = property.FindPropertyRelative("rail");
		SerializedProperty uidProp = property.FindPropertyRelative("uid");

		totalPosition.height = GetPropertyHeight(property, label);
		cursor.height = _increment;

		EditorGUI.BeginProperty(totalPosition, label, property);
		
		EditorGUI.LabelField(cursor, label);
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, railProp);
		cursor.y += _increment;

		if (railProp.objectReferenceValue != null) {
			//TODO ad sugar syntax
			Rail rail = railProp.objectReferenceValue as Rail;

			EditorGUI.PropertyField(cursor, uidProp);
			cursor.y += _increment;

			long id = uidProp.longValue;
			string msg = "Invalid UID";
			GUIStyle msgStyle = GUIStyles.LabelError;

			ControlPoint cp = new ControlPoint();
			if (rail.TryGetControlPoint(id, out cp)) {
				msg = "[" + cp.track.name + "]" + cp.label;
				msgStyle = GUIStyles.Normal;
			}

			EditorGUI.LabelField(cursor, msg, msgStyle);
		}
		else
			EditorGUI.HelpBox(cursor, "Select a rail", MessageType.Info);


		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return _increment * 4;
	}
}

