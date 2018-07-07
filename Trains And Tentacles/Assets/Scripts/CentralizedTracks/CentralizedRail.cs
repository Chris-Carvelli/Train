using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CentralizedRail : MonoBehaviour, IProcGenElement, ISerializationCallbackReceiver {
	[SerializeField]
	private IProcGenElementProperty iProgGenElementHook = new IProcGenElementProperty();

	public List<CentralizedTrack> tracks;
	public List<CentralizedSwitch> switches;

	private Dictionary<long, ControlPoint> _controlPointsDictionary;

	[SerializeField]
	private List<long> _keys;
	[SerializeField]
	private List<ControlPoint> _vals;

	private void Start() {
		iProgGenElementHook = new IProcGenElementProperty();
	}

	public void Clean() {
		foreach (CentralizedTrack track in tracks)
			track.Clean();

		foreach (CentralizedSwitch swtch in switches)
			swtch.Clean();
	}

	public void Generate() {
		foreach (CentralizedTrack track in tracks)
			track.Generate();

		foreach (CentralizedSwitch swtch in switches)
			swtch.Generate();
	}

	public void OnBeforeSerialize() {
		if (_keys == null)
			_keys = new List<long>();
		_keys.Clear();

		if (_vals == null)
			_vals = new List<ControlPoint>();
		_vals.Clear();

		_keys.AddRange(_controlPointsDictionary.Keys);
		_vals.AddRange(_controlPointsDictionary.Values);
	}

	public void OnAfterDeserialize() {
		if (_controlPointsDictionary == null)
			_controlPointsDictionary = new Dictionary<long, ControlPoint>();
		_controlPointsDictionary.Clear();

		for (int i = 0; i < _keys.Count; i++)
			_controlPointsDictionary.Add(_keys[i], _vals[i]);
	}
}

