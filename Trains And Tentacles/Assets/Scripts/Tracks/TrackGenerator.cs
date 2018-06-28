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
			DestroyImmediate(waypointsHolder.GetChild(i).gameObject);
	}

	public void Generate() {
		List<Vector3> points = new List<Vector3>();
		List<WaypointNode> waypoints = new List<WaypointNode>();

		for (int i = 0; i < controlPoints.Length; i++) {
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

				waypoints.Add(node);
			}

			float angleStep = step / (controlPoints[(i + 1) % controlPoints.Length].radius * (Mathf.PI / 2));
			a = waypoints.Last().transform.localPosition;
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

				waypoints.Add(node);

				prev = localPos;
			}
		}

		//setup children
		for (int i = 0; i < waypoints.Count; i++) {
			waypoints[i].children = new WaypointNode[1];
			waypoints[i].children[0] = waypoints[(i + 1) % waypoints.Count];
		}

		renderer.positionCount = waypoints.Count;
		renderer.SetPositions(waypoints.Select(x => x.transform.position).ToArray());
	}
}
