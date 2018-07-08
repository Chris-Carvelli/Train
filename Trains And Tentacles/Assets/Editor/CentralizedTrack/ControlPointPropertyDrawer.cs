using UnityEditor;
using UnityEngine;
using EditorGUIUtils;

//TODO cache properties
[CustomPropertyDrawer(typeof(ControlPoint))]
public class ControlPointPropertyDrawer : PropertyDrawer {
	private float _increment = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect totalPos = position;
		totalPos.height = _increment * 9;

		EditorGUI.BeginProperty(totalPos, label, property);

		Rect cursor = position;
		cursor.height = EditorGUIUtility.singleLineHeight;
		
		cursor.y += _increment;
		EditorGUI.LabelField(cursor, "Control Point Inspector", GUIStyles.Label);
		cursor.y += _increment;

		SerializedProperty rotationProp = property.FindPropertyRelative("rotation");
		SerializedProperty deviateProp = property.FindPropertyRelative("deviate");

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("label"));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("position"));
		cursor.y += _increment;

		rotationProp.quaternionValue = Quaternion.Euler(EditorGUI.Vector3Field(cursor, rotationProp.name, rotationProp.quaternionValue.eulerAngles));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("scale"));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("deviationId"));
		cursor.y += _increment;

		EditorGUI.PropertyField(cursor, property.FindPropertyRelative("direction"));
		cursor.y += _increment;

		deviateProp.boolValue = GUIControls.ToggleButton(cursor, "Normal", "Switching", deviateProp.boolValue);

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return (_increment) * 9;
	}
}

