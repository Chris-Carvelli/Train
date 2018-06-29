using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class TrackGenerator : MonoBehaviour {
	public ControlPointData root;

	public float step = 1;

	//TMP create avatar script
	public Transform waypointsHolder;

	private new LineRenderer renderer;

	//prefabs
	public WaypointNode waypointPrefab;
	public LineRenderer lineRendererPrefab;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<LineRenderer>();

		waypoints = new List<WaypointNode>();
		grey = new List<WaypointNode>();
		black = new List<WaypointNode>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Clean() {
		waypoints.Clear();
		grey.Clear();
		black.Clear();

		for (int i = 0; i < waypointsHolder.childCount; i++)
			DestroyImmediate(waypointsHolder.GetChild(i).gameObject);
	}

	private List<WaypointNode> grey = new List<WaypointNode>();
	private List<WaypointNode> black = new List<WaypointNode>();
	public void Generate() {
		GenRail(root.GetComponent<WaypointNode>());
	}

	private List<WaypointNode> waypoints;
	private void GenRail (WaypointNode curr) {
		grey.Add(curr);

		foreach(WaypointNode node in curr.children) {
			MakeRailTo(curr, node, grey.Count);
			if (!grey.Contains(node))
				GenRail(node);
		}
	}

	private void MakeRailTo(WaypointNode src, WaypointNode dst, int segmentID) {
		ControlPointData srcData = src.GetComponent<ControlPointData>();
		ControlPointData dstData = dst.GetComponent<ControlPointData>();

		Vector3 a = src.transform.localPosition;
		Vector3 b = dst.transform.localPosition;

		Vector3 dir = (b - a).normalized;

		a += dir * srcData.radius;
		b -= dir * dstData.radius;


		float distance = Vector3.Distance(a, b);

		//int midpoints = (int)(distance / step);
		int midpoints = 2;

		//straight section
		for (int j = 0; j < midpoints; j++) {
			WaypointNode node = Instantiate(waypointPrefab);
			node.name = "Waypoint" + segmentID + "_" + j;
			node.transform.SetParent(waypointsHolder);

			//node.transform.localPosition = a + dir * step * j;
			node.transform.localPosition = a + dir * distance * j;
			node.transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
			//scale?

			waypoints.Add(node);
		}

		WaypointNode start = waypoints[waypoints.Count - 2];
		start.children = new WaypointNode[1];
		start.children[0] = waypoints.Last();

	}

	/*for (int i = 0; i < controlPoints.Length; i++) {
		Vector3 a = controlPoints[i].transform.localPosition;
		Vector3 b = controlPoints[(i + 1) % controlPoints.Length].transform.localPosition;

		Vector3 dir = (b - a).normalized;

		a += dir * controlPoints[i].radius;
		b -= dir * controlPoints[(i + 1) % controlPoints.Length].radius;


		float distance = Vector3.Distance(a, b);

		int midpoints = (int)(distance / step);

		for (int j = 0; j < midpoints; j++) {
			WaypointNode node = Instantiate(waypointPrefab);
			node.name = "Waypoint" + i + "_" + j;
			node.transform.SetParent(waypointsHolder);

			node.transform.localPosition = a + dir * step * j;
			node.transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
			//scale?

			visited.Add(node);
		}

		float angleStep = step / (controlPoints[(i + 1) % controlPoints.Length].radius * (Mathf.PI / 2));
		a = visited.Last().transform.localPosition;
		b = controlPoints[(i + 1) % controlPoints.Length].transform.localPosition;
		Vector3 c = b;
		c += ((controlPoints[(i + 2) % controlPoints.Length].transform.localPosition - b) / 10) * controlPoints[(i + 1) % controlPoints.Length].radius;

		Vector3 prev = a;
		for (float j = angleStep; j < 1; j += angleStep) {
			Vector3 x = Vector3.Lerp(a, b, j);
			Vector3 y = Vector3.Lerp(b, c, j);
			Vector3 localPos = Vector3.Lerp(x, y, j);
			//dir 

			WaypointNode node = Instantiate(waypointPrefab);
			node.name = "Waypoint" + i + "_corner_" + j;
			node.transform.SetParent(waypointsHolder);

			node.transform.localPosition = localPos;

			dir = localPos - prev;
			node.transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
			//scale?

			visited.Add(node);

			prev = localPos;
		}
	}

	//setup children
	for (int i = 0; i < visited.Count; i++) {
		visited[i].children = new WaypointNode[1];
		visited[i].children[0] = visited[(i + 1) % visited.Count];
	}

	renderer.positionCount = visited.Count;
	renderer.SetPositions(visited.Select(x => x.transform.position).ToArray());*/
}
