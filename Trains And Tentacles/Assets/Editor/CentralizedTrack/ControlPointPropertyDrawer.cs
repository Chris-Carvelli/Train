using UnityEditor;
using UnityEngine;
using EditorGUIUtils;

//TODO cache properties
[CustomPropertyDrawer(typeof(ControlPoint))]
public class ControlPointPropertyDrawer : PropertyDrawer {
	private float _increment = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		
		EditorGUILayout.LabelField("Control Point Inspector", EditorStyles.boldLabel);

		Track track = property.FindPropertyRelative("track").objectReferenceValue as Track;
		EditorGUILayout.LabelField("Track: " + track.name + ". ID: " + property.FindPropertyRelative("_uid").longValue.ToString(), EditorStyles.boldLabel); 

		SerializedProperty rotationProp = property.FindPropertyRelative("rotation");
		SerializedProperty deviateProp = property.FindPropertyRelative("deviate");

		EditorGUILayout.PropertyField(property.FindPropertyRelative("label"));

		EditorGUILayout.PropertyField(property.FindPropertyRelative("position"));

		rotationProp.quaternionValue = Quaternion.Euler(EditorGUILayout.Vector3Field(rotationProp.name, rotationProp.quaternionValue.eulerAngles));

		EditorGUILayout.PropertyField(property.FindPropertyRelative("scale"));

		using (var horizontalScope = new GUILayout.HorizontalScope()) {

			EditorGUILayout.PropertyField(property.FindPropertyRelative("deviationId"), new GUIContent() {
				text = "Switch"
			});

			bool enabled = property.FindPropertyRelative("deviationId").longValue <= 0;
			using (var disableGroup = new EditorGUI.DisabledScope(enabled)) {
				EditorGUILayout.PropertyField(property.FindPropertyRelative("direction"), new GUIContent(), GUILayout.Width(70));

				deviateProp.boolValue = GUILayoutControls.Switch(enabled ? deviateProp.boolValue : false);
			}
		}

		
		//}

		//EditorGUI.PropertyField(cursor, property.FindPropertyRelative("deviationId"));
		//cursor.y += _increment;

		//if (property.FindPropertyRelative("deviationId").longValue > 0) {
		//	EditorGUI.PropertyField(cursor, property.FindPropertyRelative("direction"));
		//	cursor.y += _increment;

		//	deviateProp.boolValue = GUIControls.Switch(cursor, deviateProp.boolValue);
		//}

	}
}

