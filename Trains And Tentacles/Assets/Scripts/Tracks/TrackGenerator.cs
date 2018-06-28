using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class TrackGenerator : MonoBehaviour {
	public ControlPointData[] controlPoints;

	public float step = 1;

	//TMP create avatar script
	public Transform waypointsHolder;

	private new LineRenderer renderer;

	//prefabs
	public WaypointNode waypointPrefab;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<LineRenderer>();

		controlPoints = GetComponentsInChildren<ControlPointData>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Clean() {
		for (int i = 0; i < waypointsHolder.childCount; i++)
			DestroyImmediate(waypointsHolder.GetChild(i));
	}

	public void Generate() {
		Clean();

		List<Vector3> points = new List<Vector3>();
		List<WaypointNode> waypoints = new List<WaypointNode>();

		for (int i = 0; i < controlPoints.Length; i++) {
			Transform a = controlPoints[i].transform;
			Transform b = controlPoints[(i + 1) % controlPoints.Length].transform;

			Vector3 dir = (b.localPosition - a.localPosition).normalized;
			float distance = Vector3.Distance(a.localPosition, b.localPosition);

			a.position += dir * controlPoints[i].radius;
			b.position -= dir * controlPoints[(i + 1) % controlPoints.Length].radius;

			int midpoints = (int)(distance / step);

			for (int j = 0; j < midpoints; j++) {
				WaypointNode node = Instantiate(waypointPrefab);
				node.name = "Waypoint" + i + "_" + j;
				node.transform.SetParent(waypointsHolder);

				node.transform.localPosition = a.localPosition + dir * step * j;
				node.transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
				//scale?

				waypoints.Add(node);
			}
		}

		//setup children
		for (int i = 0; i < waypoints.Count; i++) {
			waypoints[i].children = new WaypointNode[1];
			waypoints[i].children[0] = waypoints[(i + 1) % waypoints.Count];
		}

		renderer.positionCount = controlPoints.Length;
		renderer.SetPositions(controlPoints.Select(x => x.transform.position).ToArray());
	}
}
