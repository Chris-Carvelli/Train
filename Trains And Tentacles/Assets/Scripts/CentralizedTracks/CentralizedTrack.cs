using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[Serializable]
public class ControlPoint {
	public long _uid;

	public string label;
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	public CentralizedTrack track;
	//tmp only double switches
	public long deviationId;
	public TrackDirection direction;
	public bool deviate;
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
	public long[] controlPointIds;

	[Header("Prefabs")]
	public LineRenderer rendererPrefab;

	[SerializeField]
	private CentralizedRail rail;
	private new LineRenderer renderer;

	private void Awake() {
		rail = GetComponentInParent<CentralizedRail>();
	}

	private void Start() {
		//TODO move in attribute creation
		_iProcGenHook = new IProcGenElementProperty();
		renderer = GetComponent<LineRenderer>();
	}


	//TODO move UI commands in separate class/inspector
	public void Clean () {
		renderer.positionCount = 0;
		
		for (int i = 0; i < transform.childCount; i++)
			DestroyImmediate(transform.GetChild(i).gameObject);
	}

	public void Generate() {
		Clean();

		List<Vector3> points = new List<Vector3>();
		List<LineRenderer> switches = new List<LineRenderer>();

		for (int i = 0; i < controlPointIds.Length; i++)
			if (!IsEnd(i)) {
				ControlPoint curr = rail.GetControlPoint(controlPointIds[i]);
				ControlPoint next = rail.GetControlPoint(controlPointIds[(i + 1) % controlPointIds.Length]);
				ControlPoint nextNext = rail.GetControlPoint(controlPointIds[(i + 2) % controlPointIds.Length]);

				points.AddRange(MakeRailTo(curr, next, i));

				if (!IsEnd(i + 1))
					points.AddRange(MakeCurveRail(next, curr, nextNext, i));

				if (next.deviationId > 0)
					MakeSwitch(curr, next, rail.GetControlPoint(next.deviationId), i + 1);

			}

		renderer.positionCount = points.Count;
		renderer.SetPositions(points.ToArray());
		renderer.loop = closed;
	}

	private void MakeSwitch (ControlPoint prev, ControlPoint swtch, ControlPoint dev, int i) {
		LineRenderer newRenderer = Instantiate(rendererPrefab);

		List<Vector3> points = new List<Vector3>();

		ControlPoint a = prev;
		ControlPoint b = swtch;
		ControlPoint c = dev;
		ControlPoint d = rail.GetControlPoint(dev.track.GetNextControlpoint(dev._uid, dev.direction));

		points.AddRange(MakeCurveRail(b, a, c, i));
		points.AddRange(MakeRailTo(b, c, i + 1));
		points.AddRange(MakeCurveRail(c, b, d, i + 2));

		newRenderer.positionCount = points.Count;
		newRenderer	.SetPositions(points.ToArray());
		
		newRenderer.transform.SetParent(transform, false);
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
		if (i != controlPointIds.Length - 2 || closed)
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
	/// <param name="id">index of the current point</param>
	/// <param name="dir"></param>
	/// <returns>The next control point</returns>
	public long GetNextControlpoint(long id, TrackDirection dir) {
		int i = -1;
		for (i = 0; i < controlPointIds.Length; i++)
			if (controlPointIds[i] == id)
				break;

		if (i == -1)
			return -1;

		i = i + (int)dir;

		if (!closed)
			i = Mathf.Clamp(i, 0, controlPointIds.Length - 1);

		i = i % controlPointIds.Length;

		if (i < 0)
			i += controlPointIds.Length;
		
		return controlPointIds[i];
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
		return !closed && i == controlPointIds.Length - 1;
	}
}

