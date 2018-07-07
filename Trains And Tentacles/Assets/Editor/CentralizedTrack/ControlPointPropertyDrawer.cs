using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ControlPoint))]
public class ControlPointPropertyDrawer : PropertyDrawer {
	private float _increment = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect cursor = position;
		cursor.height = EditorGUIUtility.singleLineHeight;
		SerializedProperty rotationProp = property.FindPropertyRelative("rotation");

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("label"));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("position"));
		cursor.y += _increment;

		rotationProp.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(cursor, rotationProp.name, rotationProp.quaternionValue.eulerAngles));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("scale"));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("swtch"));
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return (_increment) * 5;
	}
}

