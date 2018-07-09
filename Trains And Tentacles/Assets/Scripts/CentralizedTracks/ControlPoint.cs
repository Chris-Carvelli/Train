using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ControlPoint {
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

[Serializable]
public class ControlPointRef {
	public CentralizedRail rail;
	public long uid;

	public ControlPointRef (ControlPointRef cpr) {
		rail = cpr.rail;
		uid = cpr.uid;
	}

	public string label {
		get {
			return rail.GetControlPoint(uid).label;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.label = value;
			rail.SetControlPoint(cp);
		}
	}

	public Vector3 position {
		get {
			return rail.GetControlPoint(uid).position;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.position = value;
			rail.SetControlPoint(cp);
		}
	}

	public Quaternion rotation {
		get {
			return rail.GetControlPoint(uid).rotation;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.rotation = value;
			rail.SetControlPoint(cp);
		}
	}

	public Vector3 scale {
		get {
			return rail.GetControlPoint(uid).scale;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.scale = value;
			rail.SetControlPoint(cp);
		}
	}

	public TrackDirection direction {
		get {
			return rail.GetControlPoint(uid).direction;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.direction = value;
			rail.SetControlPoint(cp);
		}
	}

	public bool deviate {
		get {
			return rail.GetControlPoint(uid).deviate;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.deviate = value;
			rail.SetControlPoint(cp);
		}
	}

	public CentralizedTrack track {
		get {
			return rail.GetControlPoint(uid).track;
		}

		set {
			ControlPoint cp = rail.GetControlPoint(uid);
			cp.track = value;
			rail.SetControlPoint(cp);
		}
	}
}

