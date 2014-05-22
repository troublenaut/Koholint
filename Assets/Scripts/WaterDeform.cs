using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterDeform : MonoBehaviour {

	List<Vector3> baseVerticies;
	List<Vector3> workingCopy;

	// Use this for initialization
	void Start () {
		baseVerticies = new List<Vector3>( GetComponent<MeshFilter>().mesh.vertices);
		workingCopy = new List<Vector3>( baseVerticies );
	}
	
	// Update is called once per frame
	void Update () {
		for ( int index=0; index<workingCopy.Count; index++) {
			workingCopy[index] = baseVerticies[index]
				+ Vector3.up
				* Mathf.Sin (Time.time + index) * 0.2f;
		}

		GetComponent<MeshFilter>().mesh.vertices = workingCopy.ToArray();
		GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
		GetComponent<MeshFilter>().mesh.RecalculateNormals();
	}
}
