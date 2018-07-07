using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[Serializable]
public class ControlPoint {
	private long uid;

	public string label;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	//TMP only one switch at a time, switch as GameObject
	public CentralizedSwitch swtch;
}

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CentralizedTrack : MonoBehaviour, IProcGenElement {
	//TODO create attribute InterfaceControls(IType)
	[Header("Controls")]
	[SerializeField]
	private IProcGenElementProperty _iProcGenHook = new IProcGenElementProperty();

	[Header("Config")]
	public string trackName;
	public bool closed = false;
	public float step = 1;
	public ControlPoint[] controlPoints;

	private CentralizedRail rail;
	private new LineRenderer renderer;
	
	private void Start() {
		//TODO move in attribute creation
		_iProcGenHook = new IProcGenElementProperty();
		renderer = GetComponent<LineRenderer>();
		rail = GetComponentInParent<CentralizedRail>();
	}


	//TODO move UI commands in separate class/inspector
	public void Clean () {
		renderer.positionCount = 0;
	}

	public void Generate() {
		Clean();

		List<Vector3> points = new List<Vector3>();

		for (int i = 0; i < controlPoints.Length; i++)
			if (!IsEnd(i)) {
				points.AddRange(MakeRailTo(controlPoints[i], controlPoints[(i + 1) % controlPoints.Length], i));

				if (!IsEnd(i + 1))
					points.AddRange(MakeCurveRail(controlPoints[(i + 1) % controlPoints.Length], controlPoints[i], controlPoints[(i + 2) % controlPoints.Length], i));
			}

		renderer.positionCount = points.Count;
		renderer.SetPositions(points.ToArray());
		renderer.loop = closed;
	}

	public void UpdatePoint (int pointIndex) {
		throw new NotImplementedException("Not yet Implemented");
	}

	private List<Vector3> MakeRailTo(ControlPoint src, ControlPoint dst, int i) {
		Vector3 a = src.position;
		Vector3 b = dst.position;

		Vector3 dir = (b - a).normalized;

		if (i != 0 || closed)
			a += dir * src.scale.x;
		if (i != controlPoints.Length - 2 || closed)
			b -= dir * dst.scale.x;


		float distance = Vector3.Distance(a, b);

		return new List<Vector3> { a, b };
	}

	private List<Vector3> MakeCurveRail(ControlPoint curr, ControlPoint prev, ControlPoint next, int segmentID) {
		List<Vector3> ret = new List<Vector3>();
		Vector3 a = curr.position + curr.position.GetDirection(prev.position) * curr.scale.x;
		Vector3 b = curr.position;
		Vector3 c = curr.position + curr.position.GetDirection(next.position) * curr.scale.x;

		//float distance = Vector3.Distance(a, b);
		float distance = (Mathf.PI / 2) * curr.scale.x;
		int midpoints = (int)(distance / step);
		float angleStep = step / (curr.scale.x * (Mathf.PI / 2));
				
		for (int i = 0; i < midpoints - 1; i++) {
			Vector3 x = Vector3.Lerp(a, b, i * angleStep);
			Vector3 y = Vector3.Lerp(b, c, i * angleStep);
			ret.Add(Vector3.Lerp(x, y, i * angleStep));
		}

		return ret;
	}

	/// <summary>
	/// Gets the next control point in the given direction. If the track is not closed and it goes out of bounds,
	/// the current node is returned
	/// </summary>
	/// <param name="i">index of the current point</param>
	/// <param name="dir"></param>
	/// <returns>THe next control point</returns>
	public int GetNextControlpoint(int i, TrackDirection dir, out ControlPoint cp) {
		i = i + (int)dir;

		if (!closed)
			i = Mathf.Clamp(i, 0, controlPoints.Length - 1);

		i = i % controlPoints.Length;

		cp = controlPoints[i];

		return i;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="i"></param>
	/// <returns></returns>
	public bool IsCap (int i) {
		return IsStart(i) || IsEnd(i);
	}

	public bool IsStart(int i) {
		return !closed && i == 0;
	}

	public bool IsEnd(int i) {
		return !closed && i == controlPoints.Length - 1;
	}
}
