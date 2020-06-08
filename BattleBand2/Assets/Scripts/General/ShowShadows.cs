using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowShadows : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<SpriteRenderer> ().receiveShadows = true;
		this.gameObject.GetComponent<SpriteRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
