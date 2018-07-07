using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SwitchWaypoint : WaypointNode {
	public WaypointNode a;
	public List<WaypointNode> exits;

	public int direction = 0;

	private void Start() {
		SetDirection(0);
	}

	private void OnMouseOver() {
		Debug.Log("mouseOver");
		if (Input.GetButtonDown("Fire1"))
			SetDirection((direction + 1) % exits.Count);
	}

	public void SetDirection (int i) {
		if (i >= 0 && i < exits.Count) {
			direction = i;

			TrackSection[] trackSections = GetComponentsInChildren<TrackSection>();

			foreach (TrackSection trackSection in trackSections)
				trackSection.SetActive(
					trackSection.GetEnds().Item1 == a && trackSection.GetEnds().Item2 == exits[direction] ||
					trackSection.GetEnds().Item2 == a && trackSection.GetEnds().Item1 == exits[direction]);
		}
	}

	//TODO fix for n > 2, or fix n = 2
	public override ConnectionDictionary Connections {
		get {
			List<WaypointNode> connA = new List<WaypointNode> { exits[direction] };
			connA.AddRange(exits.Where((_, i) => i != direction).ToList());
			ConnectionDictionary ret = new ConnectionDictionary {
				[a] = connA,
				[exits[0]] = new List<WaypointNode> { a },
				[exits[1]] = new List<WaypointNode> { a },
			};

			return ret;
		}
	}

	public override List<WaypointNode> Children {
		get {
			return new List<WaypointNode> { a }.Concat(exits).ToList();
		}
	}

	public override WaypointNode NextChild(WaypointNode prev) {
		if (prev == a)
			return exits[direction];
		else if (exits.Contains(prev))
			return a;
		else
			return null;
	}
}
