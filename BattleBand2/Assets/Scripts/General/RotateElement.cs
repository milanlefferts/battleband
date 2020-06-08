using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateElement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.right * Time.deltaTime * 20f);
		transform.Rotate (Vector3.up * Time.deltaTime * 10f);
	}
}
