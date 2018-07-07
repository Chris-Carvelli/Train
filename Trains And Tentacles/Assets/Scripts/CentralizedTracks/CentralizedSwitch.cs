using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum TrackDirection {
	Forward = 1,
	Backward = -1
}


//TODO common base Switch and Track
[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CentralizedSwitch : MonoBehaviour, IProcGenElement {
	[SerializeField]
	private IProcGenElementProperty iProgGenElementHook = new IProcGenElementProperty();

	[Header("Runtime Info")]
	[Range(0, 1)]
	public int direction = 0;

	[Header("Config")]
	public float step = 0.1f;

	public CentralizedTrack t1;
	public CentralizedTrack t2;

	//TODO option label/index
	public long id1;
	public long id2;

	public TrackDirection next1 = TrackDirection.Forward;
	public TrackDirection next2 = TrackDirection.Forward;

	private new LineRenderer renderer;
	private CentralizedRail rail;

	private void Start() {
		iProgGenElementHook = new IProcGenElementProperty();
		renderer = GetComponent<LineRenderer>();
		rail = GetComponentInParent<CentralizedRail>();
	}

	public void Clean() {
		renderer.positionCount = 0;
	}

	public void Generate() {
		List<Vector3> points = new List<Vector3>();

		ControlPoint a;
		ControlPoint b = rail.GetControlPoint(id1);
		ControlPoint c = rail.GetControlPoint(id2);
		ControlPoint d;

		a = rail.GetControlPoint(t1.GetNextControlpoint(id1, next1));
		d = rail.GetControlPoint(t2.GetNextControlpoint(id2, next2));

		points.AddRange(MakeCurveRail(b, a, c, 0));
		points.AddRange(MakeRailTo(b, c, 1));
		points.AddRange(MakeCurveRail(c, b, d, 2));
		//points.AddRange(MakeRailTo(prevPos, t2.controlPoints[i2], 0));
		//points.AddRange(MakeCurveRail(transform.localPosition, ))

		renderer.positionCount = points.Count;
		renderer.SetPositions(points.ToArray());
	}

	private List<Vector3> MakeRailTo(ControlPoint src, ControlPoint dst, int segmentID) {
		Vector3 a = src.position;
		Vector3 b = dst.position;

		Vector3 dir = (b - a).normalized;

		a += dir * src.scale.x;
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

	private void OnMouseOver() {
		if (Input.GetButtonDown("Fire1"))
			direction = ++direction % 2;
	}
}
