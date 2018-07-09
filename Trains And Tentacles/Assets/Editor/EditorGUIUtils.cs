using UnityEditor;
using UnityEngine;
using System.Linq;

namespace EditorGUIUtils {
	public static class GUIMaterialColors {
		public static Color Enabled = new Color(0, 0.902f, 0.463f);

		public static Color Disabled = new Color(0.718f, 0.11f, 0.11f);
	}

	public static class GUIStyles {
		public static GUIStyle Normal = new GUIStyle() { };

		public static GUIStyle Editor = new GUIStyle() {
			normal = new GUIStyleState() {
				textColor = Color.white
			}
		};

		public static GUIStyle Label = new GUIStyle() {
			fontStyle = FontStyle.Bold
		};

		public static GUIStyle LabelWarn = new GUIStyle() {
			fontStyle = FontStyle.Bold,
			normal = new GUIStyleState () {
				textColor = Color.yellow
			}
		};

		public static GUIStyle LabelError = new GUIStyle() {
			fontStyle = FontStyle.Bold,
			normal = new GUIStyleState() {
				textColor = Color.red
			}
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

		public static readonly GUIStyle SwitchButtonStyleEnables = "Radio";
		public static readonly GUIStyle SwitchButtonStyleDisabled = "Radio";

		//public static GUIStyle SwitchButtonStyleEnables {
		//	get {
		//		if (_switchButtonStyleEnables == null) {
		//			Texture2D color = EditorGUIUtility.whiteTexture;
		//			color.SetPixels(EditorGUIUtility.whiteTexture.GetPixels().Select(x => GUIMaterialColors.Enabled).ToArray());

		//			_switchButtonStyleEnables = new GUIStyle(EditorStyles.radioButton) {
		//				normal = new GUIStyleState() {
		//					background = color
		//				}
		//			};
		//		}

		//		return _switchButtonStyleEnables;
		//	}
		//}

		//public static GUIStyle SwitchButtonStyleDisabled {
		//	get {
		//		if (_switchButtonStyleDisabled == null) {
		//			Texture2D color = EditorGUIUtility.whiteTexture;
		//			color.SetPixels(EditorGUIUtility.whiteTexture.GetPixels().Select(x => GUIMaterialColors.Disabled).ToArray());

		//			_switchButtonStyleDisabled = new GUIStyle(EditorStyles.radioButton) {
		//				normal = new GUIStyleState() {
		//					background = color
		//				}
		//			};
		//		}

		//		return _switchButtonStyleDisabled;
		//	}
		//}
	}

	public static class GUIControls {
		public static bool ToggleButton(Rect position, string normalText, string toggleText, bool value) {
			return GUI.Button(position, value ? toggleText : normalText, value ? GUIStyles.ToggleButtonStyleToggled : GUIStyles.ToggleButtonStyleNormal) ? !value : value;
		}

		public static bool ToggleButton(Rect position, Texture image, bool value) {
			return GUI.Button(position, image, value ? GUIStyles.ToggleButtonStyleToggled : GUIStyles.ToggleButtonStyleNormal) ? !value : value;
		}

		public static bool Switch(Rect position, bool value) {
			Color oldColor = GUI.color;

			GUI.color = value ? GUIMaterialColors.Enabled : GUIMaterialColors.Disabled;
			bool ret = GUI.Button(position, "", "Radio") ? !value : value;

			GUI.color = oldColor;
			return ret;
		}
	}

	public static class GUILayoutControls {

		public static bool Switch(bool value) {
			Color oldColor = GUI.color;

			GUI.color = value ? GUIMaterialColors.Enabled : GUIMaterialColors.Disabled;
			bool ret = GUILayout.Button("", "Radio") ? !value : value;

			GUI.color = oldColor;
			return ret;
		}
	}
}