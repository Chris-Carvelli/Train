using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3ExtensionMethods {

	/// <summary>
	/// Returns the direction towars the target point
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public static Vector3 GetDirection(this Vector3 a, Vector3 b) {
		return (b - a).normalized;
	}
}
