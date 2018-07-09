using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using EditorGUIUtils;


public static class ControlPointEditorExtensions {
	public static ControlPoint DeserializeControlPoint(SerializedProperty prop) {
		return new ControlPoint {
			label = prop.FindPropertyRelative("label").stringValue,
			position = prop.FindPropertyRelative("position").vector3Value,
			rotation = prop.FindPropertyRelative("rotation").quaternionValue,
			scale = prop.FindPropertyRelative("scale").vector3Value,
			deviationId = prop.FindPropertyRelative("deviationId").longValue,
			direction = (TrackDirection)prop.FindPropertyRelative("direction").enumValueIndex,
			deviate = prop.FindPropertyRelative("deviate").boolValue
		};
	}

	public static void SerializeControlPoint(this ControlPoint cp, ref SerializedProperty prop) {
		prop.FindPropertyRelative("label").stringValue = cp.label;

		prop.FindPropertyRelative("position").vector3Value = cp.position;

		prop.FindPropertyRelative("rotation").quaternionValue = cp.rotation;

		prop.FindPropertyRelative("scale").vector3Value = cp.scale;

		prop.FindPropertyRelative("deviationId").longValue = cp.deviationId;

		prop.FindPropertyRelative("direction").enumValueIndex = (int)cp.direction;

		prop.FindPropertyRelative("deviate").boolValue = cp.deviate;
	}
}


[CustomEditor(typeof(Track))]
public class CentralizedTrackEditor : Editor {
	private Transform transform;

	private Rail rail;
	private ControlPointEditor cachedEditor;

	private SerializedProperty controls;
	private SerializedProperty closed;
	private SerializedProperty step;
	private SerializedProperty controlPoints;

	private SerializedProperty rendererPrefab;

	private ReorderableList controlPointList;
	
	private int _newFocusedControlPointIndex;

	//shader members

	private static int _focusedControlPointIndex;
	private static bool _simplifiedPreview;

	private void OnEnable() {
		_focusedControlPointIndex = 0;

		rail = serializedObject.FindProperty("rail").objectReferenceValue as Rail;

		cachedEditor = CreateEditor(rail, typeof(ControlPointEditor)) as ControlPointEditor;


		controls = serializedObject.FindProperty("_iProcGenHook");

		closed = serializedObject.FindProperty("closed");
		step = serializedObject.FindProperty("step");
		controlPoints = serializedObject.FindProperty("controlPointIds");

		rendererPrefab = serializedObject.FindProperty("rendererPrefab");

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
				idProp.longValue = rail.NewControlPoint(target as Track);
			},

			onRemoveCallback = (ReorderableList list) => {
				rail.RemoveControlPoint(controlPoints.GetArrayElementAtIndex(list.index).longValue);
				controlPoints.DeleteArrayElementAtIndex(list.index);
			},

			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				if (isFocused)
					_newFocusedControlPointIndex = index;

				SerializedProperty prop = controlPointList.serializedProperty.GetArrayElementAtIndex(index);
				long elementId = prop.longValue;
				var element = rail.GetControlPoint(elementId);
				string text = element.label;
				
				if (element.deviationId > 0) {
					var deviationElement = rail.GetControlPoint(element.deviationId);
					text += " => [" + deviationElement.track.name + "] " + deviationElement.label;
					
					bool res = GUIControls.Switch(
						new Rect(rect.width - 35, rect.y, 35, EditorGUIUtility.singleLineHeight),
						element.deviate);

					if (res != element.deviate) {
						element.deviate = res;
						rail.SetControlPoint(element);
					}
				}

				EditorGUI.LabelField(
				new Rect(rect.x, rect.y, rect.width - 35, EditorGUIUtility.singleLineHeight),
				text);
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
		EditorGUILayout.PropertyField(rendererPrefab);

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

		_simplifiedPreview = EditorGUILayout.ToggleLeft("Simplified preview", _simplifiedPreview);

		serializedObject.ApplyModifiedProperties();
	}

	public void OnSceneGUI() {
		transform = (target as MonoBehaviour).transform;

		serializedObject.Update();
		EditorGUI.BeginChangeCheck();

		for (int i = 0; i < controlPoints.arraySize; i++) {

			SerializedProperty currProperty = controlPoints.GetArrayElementAtIndex(i);
			SerializedProperty nextProperty = controlPoints.GetArrayElementAtIndex((i + 1) % controlPoints.arraySize);

			ControlPoint curr, next;
			
			curr = rail.GetControlPoint(currProperty.longValue);
			next = rail.GetControlPoint(nextProperty.longValue);

			string label = curr.label + "[" +  curr._uid.ToString() + "]";
			Handles.Label(transform.TransformPoint(curr.position), label, GUIStyles.Editor);

			if (i == _focusedControlPointIndex)
				DrawToolAndUpdate(ref curr);

			rail.SetControlPoint(curr);


			if (!IsEnd(i))
				if (_simplifiedPreview)
					DrawLine(curr.position, next.position, Color.green);
				else {
					SerializedProperty nextNextProperty = controlPoints.GetArrayElementAtIndex((i + 2) % controlPoints.arraySize);
					ControlPoint NextNext = rail.GetControlPoint(nextNextProperty.longValue);

					MakeRailTo(curr, next, i, Color.green);

					if (!IsEnd(i + 1))
						MakeCurveRail(next, curr, NextNext, i, Color.green);
				}


			if (curr.deviationId > 0)
				if (_simplifiedPreview)
					DrawLine(curr.position, rail.GetControlPoint(curr.deviationId).position, Color.yellow);
				else {
					if (i - 1 < 0 && !closed.boolValue)
						throw new System.NotImplementedException("canno create switch on first control point (control on prev needs to be added here)");
					SerializedProperty prevProperty = controlPoints.GetArrayElementAtIndex((i - 1) % controlPoints.arraySize);
					ControlPoint prev = rail.GetControlPoint(prevProperty.longValue);

					MakeSwitch(prev, curr, rail.GetControlPoint(curr.deviationId), i + 1, Color.yellow);
				}
		}

		if (EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	private void DrawLine(Vector3 curr, Vector3 next, Color color, bool clickable = true) {
		using (new Handles.DrawingScope(color)) {
			Handles.DrawLine(transform.TransformPoint(curr), transform.TransformPoint(next));

			Vector3 pos = curr + (next - curr).normalized * (Vector3.Distance(next, curr) / 2);
			pos = transform.TransformPoint(pos);

			Vector3 lookVector = next - curr;

			if (lookVector != Vector3.zero && clickable)
				if (Handles.Button(pos, Quaternion.LookRotation(lookVector, transform.up), .2f, .2f, Handles.CubeHandleCap))
					Debug.Log("Clicked");
		}
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
				curr.scale.x = Handles.RadiusHandle(transform.rotation * curr.rotation, transform.TransformPoint(curr.position), curr.scale.x);
				break;
		}
	}

	//TODO make unique point generation system with custom callbacks to render the points (functional)
	private void MakeSwitch(ControlPoint prev, ControlPoint swtch, ControlPoint dev, int i, Color color) {
		List<Vector3> points = new List<Vector3>();

		ControlPoint a = prev;
		ControlPoint b = swtch;
		ControlPoint c = dev;
		ControlPoint d = rail.GetControlPoint(dev.track.GetNextControlPoint(dev._uid, swtch._uid, ref swtch.direction));

		MakeCurveRail(b, a, c, i, color);
		MakeRailTo(b, c, i + 1, color);
		MakeCurveRail(c, b, d, i + 2, color);

		//render handles
	}

	private void MakeRailTo(ControlPoint src, ControlPoint dst, int i, Color color) {
		Vector3 a = src.position;
		Vector3 b = dst.position;

		Vector3 dir = (b - a).normalized;

		if (i != 0 || closed.boolValue)
			a += dir * src.scale.x;
		if (i != controlPoints.arraySize - 2 || closed.boolValue)
			b -= dir * dst.scale.x;


		float distance = Vector3.Distance(a, b);

		DrawLine(a, b, color);
	}

	private void MakeCurveRail(ControlPoint curr, ControlPoint prev, ControlPoint next, int segmentID, Color color) {
		Vector3 a = curr.position + curr.position.GetDirection(prev.position) * curr.scale.x;
		Vector3 b = curr.position;
		Vector3 c = curr.position + curr.position.GetDirection(next.position) * curr.scale.x;

		//float distance = Vector3.Distance(a, b);
		float distance = (Mathf.PI / 2) * curr.scale.x;
		int midpoints = (int)(distance / step.floatValue);
		float angleStep = step.floatValue / (curr.scale.x * (Mathf.PI / 2));

		Vector3 prevPoint = a;
		for (int i = 0; i < midpoints + 2; i++) {
			Vector3 x = Vector3.Lerp(a, b, i * angleStep);
			Vector3 y = Vector3.Lerp(b, c, i * angleStep);
			Vector3 nextPoint = Vector3.Lerp(x, y, i * angleStep);
			DrawLine(prevPoint, nextPoint, color, false);
			prevPoint = nextPoint;
		}
	}

	public bool IsCap(int i) {
		return IsStart(i) || IsEnd(i);
	}

	public bool IsStart(int i) {
		return !closed.boolValue && i == 0;
	}

	public bool IsEnd(int i) {
		return !closed.boolValue && i == controlPoints.arraySize - 1;
	}
}
