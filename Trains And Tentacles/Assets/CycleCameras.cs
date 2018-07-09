using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleCameras : MonoBehaviour {
	public List<Camera> cameras;
	
	void Update () {
		for (int i = 0; i < cameras.Count && i < 10; i++)
			if (Input.GetKeyDown("[" + (i + 1).ToString() + "]"))
				SetCamera(i);
	}

	public void SetCamera (int index) {
		foreach (Camera camera in cameras)
			camera.enabled = false;

		cameras[index].enabled = true;
	}
}
