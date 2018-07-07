using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public static class ControlPointEditorExtensions {
	public static ControlPoint DeserializeControlPoint(SerializedProperty prop) {
		return new ControlPoint {
			label = prop.FindPropertyRelative("label").stringValue,
			position = prop.FindPropertyRelative("position").vector3Value,
			rotation = prop.FindPropertyRelative("rotation").quaternionValue,
			scale = prop.FindPropertyRelative("scale").vector3Value,
			swtch = prop.FindPropertyRelative("swtch").objectReferenceValue as CentralizedSwitch
		};
	}

	public static void SerializeControlPoint(this ControlPoint cp, ref SerializedProperty prop) {
	prop.FindPropertyRelative("label").stringValue = cp.label;

	prop.FindPropertyRelative("position").vector3Value = cp.position;

	prop.FindPropertyRelative("rotation").quaternionValue = cp.rotation;

	prop.FindPropertyRelative("scale").vector3Value = cp.scale;

	prop.FindPropertyRelative("swtch").objectReferenceValue = cp.swtch;
	}
}


[CustomEditor(typeof(CentralizedTrack))]
public class CentralizedTrackEditor : Editor {
	private Transform transform;

	private CentralizedRail rail;
	private ControlPointEditor cachedEditor;

	private SerializedProperty trackName;
	private SerializedProperty controls;
	private SerializedProperty closed;
	private SerializedProperty step;
	private SerializedProperty controlPoints;

	private ReorderableList controlPointList;
	
	private int _newFocusedControlPointIndex;

	//shader members

	private static int _focusedControlPointIndex;

	private void OnEnable() {
		_focusedControlPointIndex = 0;

		rail = serializedObject.FindProperty("rail").objectReferenceValue as CentralizedRail;

		cachedEditor = CreateEditor(rail, typeof(ControlPointEditor)) as ControlPointEditor;


		controls = serializedObject.FindProperty("_iProcGenHook");

		trackName = serializedObject.FindProperty("trackName");
		closed = serializedObject.FindProperty("closed");
		step = serializedObject.FindProperty("step");
		controlPoints = serializedObject.FindProperty("controlPointIds");

		controlPointList = new ReorderableList(serializedObject, controlPoints, true, true, true, true) {

			// Draw header label
			drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Control Points");
			},

			elementHeightCallback = (int index) => {
				var elementHeight = EditorGUIUtility.singleLineHeight;
				// optional, depending on the situation in question and the defaults you like
				// you may want to subtract the margin out in the drawElementCallback before drawing
				var margin = EditorGUIUtility.standardVerticalSpacing;
				return elementHeight + margin;
			},

			onAddCallback = (ReorderableList list) => {
				controlPoints.InsertArrayElementAtIndex(controlPoints.arraySize);
				var idProp = controlPoints.GetArrayElementAtIndex(controlPoints.arraySize - 1);
				idProp.longValue = rail.NewControlPoint();
			},

			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				if (isFocused)
					_newFocusedControlPointIndex = index;

				long elementId = controlPointList.serializedProperty.GetArrayElementAtIndex(index).longValue;
				var element = rail.GetControlPoint(elementId);

				EditorGUI.LabelField(
					new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
					element.label);
			}
		};
	}

	public override void OnInspectorGUI() {
		if (_focusedControlPointIndex == controlPointList.count)
			_focusedControlPointIndex--;

		serializedObject.Update();

		EditorGUILayout.PropertyField(controls);
		EditorGUILayout.PropertyField(closed);
		EditorGUILayout.PropertyField(step);

		if (_focusedControlPointIndex != -1) {
			long id = controlPoints.GetArrayElementAtIndex(_focusedControlPointIndex).longValue;

			cachedEditor.OnInspectorGUI(id);
		}
		else
			EditorGUILayout.HelpBox("Select a Control point in the list", MessageType.Info);

		controlPointList.DoLayoutList();
		if (_newFocusedControlPointIndex != _focusedControlPointIndex) {
			_focusedControlPointIndex = _newFocusedControlPointIndex;
			SceneView.RepaintAll();
		}

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI() {
		transform = (serializedObject.targetObject as MonoBehaviour).transform;

		serializedObject.Update();
		EditorGUI.BeginChangeCheck();

		for (int i = 0; i < controlPoints.arraySize; i++) {

			SerializedProperty currProperty = controlPoints.GetArrayElementAtIndex(i);
			SerializedProperty nextProperty = controlPoints.GetArrayElementAtIndex((i + 1) % controlPoints.arraySize);

			ControlPoint curr = rail.GetControlPoint(currProperty.longValue);
			ControlPoint next = rail.GetControlPoint(nextProperty.longValue);

			Handles.Label(transform.TransformPoint(curr.position), curr.label);

			if (i == _focusedControlPointIndex)
				DrawToolAndUpdate(ref curr);

			rail.SetControlPoint(curr);
			if ((i == controlPoints.arraySize - 1 && closed.boolValue) || i < controlPoints.arraySize - 1)
				DrawLine(curr.position, next.position);
		}

		if (EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	private void DrawLine(Vector3 curr, Vector3 next) {
		Handles.DrawLine(transform.TransformPoint(curr), transform.TransformPoint(next));

		Vector3 pos = curr + (next - curr).normalized * (Vector3.Distance(next, curr) / 2);
		pos = transform.TransformPoint(pos);

		Vector3 lookVector = next - curr;

		if (lookVector != Vector3.zero)
			if (Handles.Button(pos, Quaternion.LookRotation(lookVector, transform.up), .2f, .2f, Handles.CubeHandleCap))
				Debug.Log("Clicked"); ;
	}

	private void DrawToolAndUpdate (ref ControlPoint curr) {
		switch (Tools.current) {
			case Tool.Move:
				curr.position = transform.InverseTransformPoint(Handles.PositionHandle(transform.TransformPoint(curr.position), transform.rotation * curr.rotation));
				break;
			case Tool.Rotate:
				curr.rotation = Quaternion.Inverse(transform.rotation) * Handles.RotationHandle(transform.rotation * curr.rotation, transform.TransformPoint(curr.position));
				break;
			case Tool.Scale:
				//curr.scale = Handles.ScaleHandle(curr.scale, transform.TransformPoint(curr.position), transform.rotation * curr.rotation, 1f);
				curr.scale.x = Handles.RadiusHandle(transform.rotation * curr.rotation, transform.TransformPoint(curr.position), curr.scale.x);
				break;
		}
	}
}
