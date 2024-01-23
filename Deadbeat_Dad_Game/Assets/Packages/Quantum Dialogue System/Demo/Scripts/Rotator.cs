using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public Vector3 rotation = new Vector3(0, 10, 0);

	void Update () {
		this.transform.Rotate (rotation * Time.deltaTime);
	}
}
