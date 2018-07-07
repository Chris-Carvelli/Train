using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveWaypoint : WaypointNode {
	public WaypointNode a, b;

	public override ConnectionDictionary Connections {
		get {
			ConnectionDictionary ret = new ConnectionDictionary {
				[a] = new List<WaypointNode> { b },
				[b] = new List<WaypointNode> { a }
			};

			return ret; 
		}
	}

	public override List<WaypointNode> Children {
		get {
			return new List<WaypointNode> { a, b };
		}
	}

	public override WaypointNode NextChild(WaypointNode prev) {
		if (prev == a)
			return b;
		else if (prev == b)
			return a;
		else
			return null;
	}

	public override void DoSwitch(WaypointNode swtc) { }
}
