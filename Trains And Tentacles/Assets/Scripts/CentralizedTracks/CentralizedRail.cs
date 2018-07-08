using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CentralizedRail : MonoBehaviour, IProcGenElement, ISerializationCallbackReceiver {
	[SerializeField]
	private IProcGenElementProperty iProgGenElementHook = new IProcGenElementProperty();

	public List<CentralizedTrack> tracks;
	public List<CentralizedSwitch> switches;

	private Dictionary<long, ControlPoint> _controlPointsDictionary;
	//tmp as long, use GetElementID
	private long newId = 1;

	[SerializeField]
	private List<long> _keys;
	[SerializeField]
	private List<ControlPoint> _vals;

	private void Awake() {
		if (_controlPointsDictionary == null)
			_controlPointsDictionary = new Dictionary<long, ControlPoint>();
	}

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

	public ControlPoint GetControlPoint(long id) {
		return _controlPointsDictionary[id];
	}
	
	public void SetControlPoint (ControlPoint cp) {
		if (cp._uid == 0)
			cp._uid = newId++;

		_controlPointsDictionary[cp._uid] = cp;
	}

	public long NewControlPoint(ControlPoint cp) {
		cp._uid = cp._uid = newId++;

		_controlPointsDictionary[cp._uid] = cp;

		return cp._uid;
	}

	public long NewControlPoint(CentralizedTrack track) {
		return NewControlPoint(new ControlPoint() {
			rotation = Quaternion.identity,
			track = track
		});
	}
}

