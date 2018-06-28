using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class ProceduralRobotArm : MonoBehaviour {
	public int nJoints = 3;
	public float sectionLength = 1;

	public bool debugging = false;

	public RobotJoint bonePrefab;

	private new SkinnedMeshRenderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SkinnedMeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (debugging)
			DrawDebug();
	}

	public void Clean() {
		RobotJoint[] bones = GetComponentsInChildren<RobotJoint>();

		foreach (RobotJoint bone in bones)
			DestroyImmediate(bone.gameObject);
	}

	public void Generate() {
		Clean();

		Transform[] bones = new Transform[nJoints];
		Matrix4x4[] bindPoses = new Matrix4x4[nJoints];

		Transform curr = transform;
		curr.localPosition = Vector3.zero;
		for (int i = 0; i < bones.Length; i++) {
			RobotJoint joint = Instantiate(bonePrefab);
			joint.transform.GetChild(0).localScale = Vector3.one * sectionLength;
			//TMP
			switch (i % 3) {
				case 0:
					joint.axis = Vector3.up;
					break;
				case 1:
					joint.axis = Vector3.forward;
					break;
				case 2:
					joint.axis = Vector3.right;
					break;
			}


			bones[i] = joint.transform;
			bones[i].name = "RobotJoint" + i;
			bones[i].SetParent(curr, true);

			bones[i].localRotation = curr.localRotation;
			bones[i].localScale = curr.localScale;
			bones[i].localPosition = curr.up * sectionLength;

			bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;

			curr = bones[i];
		}
		renderer.bones = bones;

	}

	private void DrawDebug() {
		Transform curr = transform;
			foreach (Transform bone in renderer.bones) 
				if (bone != null) {
					Debug.DrawLine(curr.position, bone.position);
					curr = bone;
				}
	}
}

