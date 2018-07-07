using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

[ExecuteInEditMode]
public class WaypointNode : MonoBehaviour {
	public float radius;

	protected ConnectionDictionary _connections;
	public virtual ConnectionDictionary Connections {
		get {
			return _connections;
		}
	}

	public virtual List<WaypointNode> Children {
		get {
			return _connections.dictionary.Keys.ToList();
		}
	}

	public virtual WaypointNode NextChild(WaypointNode prev) {
		return _connections[prev][0];
	}

	public virtual void DoSwitch(WaypointNode swtch) {
		WaypointNode node = _connections[swtch][0];
		_connections[swtch].RemoveAt(0);
		_connections[swtch].Add(node);
	}

	[Serializable]
	public class Exits : ScriptableObject {
		[SerializeField]
		public List<WaypointNode> _exits = new List<WaypointNode>();

		public IEnumerator GetEnumerator() => _exits.GetEnumerator();

		public WaypointNode this[int i] {
			get {
				return _exits[i];
			}

			set {
				_exits[i] = value;
			}
		}

		public void RemoveAt(int i) {
			_exits.RemoveAt(i);
		}

		public void Add(WaypointNode node) {
			_exits.Add(node);
		}
	}

	[Serializable]
	public class ConnectionDictionary : SerializableDictionary<WaypointNode, List<WaypointNode>> {}
}
