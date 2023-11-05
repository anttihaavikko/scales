using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoneToLine : MonoBehaviour {

	public bool bezier = false;
	public Anima2D.Bone2D[] bones;
	private LineRenderer line;
    public float lineZ = 0f;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {

        line.SetPosition (0, new Vector3(transform.position.x, transform.position.y, lineZ));

		if (bezier) {
			for (int i = 1; i < line.positionCount; i++) {
				// B(t) = (1-t)^2P0 + 2(1-t)tP1 + t2P2 , 0 < t < 1

				float t = (float)i / (float)line.positionCount;
				Vector3 p = Mathf.Pow (1 - t, 2) * transform.position + 2 * (1 - t) * t * bones [0].endPosition + Mathf.Pow (t, 2) * bones [1].endPosition;
				line.SetPosition (i, p);
			}
		} else {

            for (int i = 0; i < bones.Length; i++) {
                line.SetPosition (i + 1, new Vector3(bones[i].endPosition.x, bones[i].endPosition.y, lineZ));
			}
		}
	}
}
