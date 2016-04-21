using UnityEngine;
using System.Collections;

public class rotationStatic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        this.transform.eulerAngles = Vector3.zero;
	}
}
