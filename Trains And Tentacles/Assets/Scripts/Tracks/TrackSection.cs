using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrackSection : MonoBehaviour {
	public float activeWidth = 0.2f;
	public float inactiveWidth = 0.1f;
	public WaypointNode a, b;
	public TrackGenerator generator;

	protected new LineRenderer renderer;

	private bool _active = false;

	void Awake() {
		Init();
	}

	public void Init() {
		renderer = GetComponent<LineRenderer>();
	}

	public void SetEnds(WaypointNode a, WaypointNode b) {
		this.a = a;
		this.b = b;
	}

	public Tuple<WaypointNode, WaypointNode> GetEnds () {
		return new Tuple<WaypointNode, WaypointNode>(a, b);
	}

	public void SetActive(bool active) {
		_active = active;

		renderer.sharedMaterial = _active ? generator.activematerial : generator.inactiveMaterial;
		renderer.sortingLayerName = _active ? "ActiveTrack" : "InactiveTrack";
		Vector3[] positions = new Vector3[100];

		renderer.GetPositions(positions);
		//renderer.SetPositions(positions.Select(x => new Vector3(x.x, x.y - (_active ? 0 : 0.001f), x.z)).ToArray());
		renderer.startWidth = renderer.endWidth = _active ? activeWidth : inactiveWidth;
	}

	public void SetPositions(Vector3[] pos) {
		renderer.positionCount = pos.Length;
		renderer.SetPositions(pos);
	}
}

