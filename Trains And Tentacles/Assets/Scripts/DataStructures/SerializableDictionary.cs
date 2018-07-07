using System.Collections;
using System.Collections.Generic;

using UnityEngine;

abstract public class SerializableDictionary<K, V> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<K, V>> {
	[SerializeField]
	private K[] keys;
	[SerializeField]
	private V[] values;

	public Dictionary<K, V> dictionary = new Dictionary<K, V>();

	static public T New<T>() where T : SerializableDictionary<K, V>, new() {
		var result = new T();
		result.dictionary = new Dictionary<K, V>();
		return result;
	}

	public V this[K key] {
		get {
			return dictionary[key];
		}

		set {
			dictionary[key] = value;
		}
	}

	public void OnAfterDeserialize() {
		var c = keys.Length;
		dictionary = new Dictionary<K, V>(c);
		for (int i = 0; i < c; i++) {
			dictionary[keys[i]] = values[i];
		}
		keys = null;
		values = null;
	}

	public void OnBeforeSerialize() {
		if (dictionary == null) {
			Debug.LogWarning("Dictionary null");
			dictionary = new Dictionary<K, V>();
		}

		var c = dictionary.Count;
		keys = new K[c];
		values = new V[c];
		int i = 0;
		using (var e = dictionary.GetEnumerator())
			while (e.MoveNext()) {
				var kvp = e.Current;
				keys[i] = kvp.Key;
				values[i] = kvp.Value;
				i++;
			}
	}

	public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
		return dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return dictionary.GetEnumerator();
	}
}