using UnityEditor;
using UnityEngine;

namespace EditorGUIUtils {
	public static class GUIStyles {
		public static GUIStyle Label = new GUIStyle() {
			fontStyle = FontStyle.Bold
		};

		public static readonly GUIStyle ToggleButtonStyleNormal = "Button";
		private static GUIStyle _toggleButtonStyleToggled = null;

		public static GUIStyle ToggleButtonStyleToggled {
			get {
				if (_toggleButtonStyleToggled == null)
					_toggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal) {
						normal = new GUIStyleState() {
							background = ToggleButtonStyleNormal.active.background
						}
					};
				return _toggleButtonStyleToggled;
			}
		}
	}

	public static class GUIControls {
		public static bool ToggleButton(Rect position, string normalText, string toggleText, bool value) {

			return GUI.Button(position, value ? toggleText : normalText, value ? GUIStyles.ToggleButtonStyleToggled : GUIStyles.ToggleButtonStyleNormal) ? !value : value;
		}
	}
}