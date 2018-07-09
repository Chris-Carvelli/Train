using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using EditorGUIUtils;

[CustomEditor(typeof(CentralizedRail))]
public class CentralizedRailEditor : Editor {
	private SerializedProperty trackListProp;

	private SerializedProperty controlPointKeys;
	private SerializedProperty controlPointVals;

	private ControlPointEditor cachedControlPointEditor;
	private List<CentralizedTrackEditor> cachedTracksEditors;

	private ReorderableList controlPointList;

	private int _newFocusedControlPointIndex;

	//shader members

	private static int _focusedControlPointIndex;

	private void OnEnable() {
		trackListProp = serializedObject.FindProperty("tracks");
		controlPointKeys = serializedObject.FindProperty("_keys");
		controlPointVals = serializedObject.FindProperty("_vals");

		CentralizedRail rail = (target as CentralizedRail);

		cachedControlPointEditor = CreateEditor(rail, typeof(ControlPointEditor)) as ControlPointEditor;

		cachedTracksEditors = new List<CentralizedTrackEditor>();
		for (int i = 0; i < trackListProp.arraySize; i++) {
			SerializedProperty track = trackListProp.GetArrayElementAtIndex(i);
			cachedTracksEditors.Add(CreateEditor(track.objectReferenceValue, typeof(CentralizedTrackEditor)) as CentralizedTrackEditor);
		}

		controlPointList = new ReorderableList(serializedObject, controlPointKeys, false, true, false, true) {

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
				controlPointKeys.InsertArrayElementAtIndex(controlPointKeys.arraySize);
				var idProp = controlPointKeys.GetArrayElementAtIndex(controlPointKeys.arraySize - 1);
				idProp.longValue = rail.NewControlPoint(target as CentralizedTrack);
			},

			onRemoveCallback = (ReorderableList list) => {
				rail.RemoveControlPoint(controlPointKeys.GetArrayElementAtIndex(list.index).longValue);
			},

			drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				if (isFocused)
					_newFocusedControlPointIndex = index;

				SerializedProperty prop = controlPointList.serializedProperty.GetArrayElementAtIndex(index);
				long elementId = prop.longValue;
				var element = rail.GetControlPoint(elementId);
				string text = "[" + element.track.name + "] " +  element.label;

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
		base.OnInspectorGUI();

		if (_focusedControlPointIndex != -1) {
			long id = controlPointKeys.GetArrayElementAtIndex(_focusedControlPointIndex).longValue;

			cachedControlPointEditor.OnInspectorGUI(id);
		}
		else
			EditorGUILayout.HelpBox("Select a Control point in the list", MessageType.Info);

		if (_focusedControlPointIndex == controlPointList.count)
			_focusedControlPointIndex--;

		controlPointList.DoLayoutList();
		if (_newFocusedControlPointIndex != _focusedControlPointIndex) {
			_focusedControlPointIndex = _newFocusedControlPointIndex;
			SceneView.RepaintAll();
		}
	}

	private void OnSceneGUI() {

		Tool lastTool = Tools.current;
		Tools.current = Tool.None;

		cachedTracksEditors.ForEach(x => x.OnSceneGUI());

		Tools.current = lastTool;
	}
}
