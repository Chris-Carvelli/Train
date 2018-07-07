using System;
using System.Collections.Generic;
using UnityEngine;

public interface IProcGenElement {

	void Clean();
	void Generate();
}

[Serializable]
public struct IProcGenElementProperty { }