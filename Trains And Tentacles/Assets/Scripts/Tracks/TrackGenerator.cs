using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class TrackGenerator : MonoBehaviour {
	public WaypointNode root;

	[Header("Config")]
	public float step = 1;
	public Material activematerial;
	public Material inactiveMaterial;
	private new LineRenderer renderer;

	[Header("Prefabs")]
	public Transform renderersHolder;
	public TrackSection trackSectionPrefab;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<LineRenderer>();

		waypoints = new List<Vector3>();
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

		LineRenderer[] renderers = GetComponentsInChildren<LineRenderer>();
		foreach (LineRenderer renderer in renderers)
			DestroyImmediate(renderer.gameObject);
	}

	private List<WaypointNode> grey = new List<WaypointNode>();
	private List<WaypointNode> black = new List<WaypointNode>();
	public void Generate() {
		GenRail(root.GetComponent<WaypointNode>());
	}

	private List<Vector3> waypoints;
	private void GenRail (WaypointNode curr) {
		grey.Add(curr);

		foreach(WaypointNode node in curr.Children) {
			if (!grey.Contains(node))
				GenRail(node);
			if (!black.Contains(node))
				MakeRailTo(curr, node, grey.Count);
		}

		foreach (KeyValuePair<WaypointNode, List<WaypointNode>> connections in curr.Connections)
			foreach (WaypointNode node in connections.Value)
				MakeCurveRail(curr, connections.Key, node, grey.Count);

		black.Add(curr);
	}

	private void MakeRailTo(WaypointNode src, WaypointNode dst, int segmentID) {
		Vector3 a = src.transform.localPosition;
		Vector3 b = dst.transform.localPosition;

		Vector3 dir = (b - a).normalized;

		a += dir * src.radius;
		b -= dir * dst.radius;


		float distance = Vector3.Distance(a, b);

		waypoints.Add(a);
		waypoints.Add(b);

		TrackSection tsp = Instantiate(trackSectionPrefab);
		tsp.SetEnds(src, dst);
		tsp.Init();
		tsp.transform.SetParent(renderersHolder);
		tsp.generator = this;
		tsp.SetPositions(new List<Vector3> { a, b }.ToArray());
	}

	private void MakeCurveRail(WaypointNode curr, WaypointNode prev, WaypointNode next, int segmentID) {
		Vector3 a = curr.transform.localPosition + GetDirection(curr.transform, prev.transform) * curr.radius;
		Vector3 b = curr.transform.localPosition;
		Vector3 c = curr.transform.localPosition + GetDirection(curr.transform, next.transform) * curr.radius;

		//float distance = Vector3.Distance(a, b);
		float distance = (Mathf.PI / 2) * curr.radius;
		int midpoints = (int)(distance / step);
		float angleStep = step / (curr.radius * (Mathf.PI / 2));

		List<Vector3> points = new List<Vector3>();

		TrackSection tsp = Instantiate(trackSectionPrefab);
		tsp.SetEnds(prev, next);
		tsp.Init();

		for (int i = 0; i < midpoints; i++) {
			Vector3 x = Vector3.Lerp(a, b, i * angleStep);
			Vector3 y = Vector3.Lerp(b, c, i * angleStep);
			points.Add(Vector3.Lerp(x, y, i * angleStep));
		}

		tsp.transform.SetParent(curr.transform);
		tsp.generator = this;
		tsp.SetPositions(points.ToArray());
	}

	private Vector3 GetDirection(Transform a, Transform b) {
		return a.localPosition.GetDirection(b.localPosition);
	}
}

