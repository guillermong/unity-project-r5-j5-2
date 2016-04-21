using UnityEngine;
using System.Collections;

public class RangeNodeRotation : MonoBehaviour {

    private RaycastHit hit;
    public LayerMask mask;

    private Color inicialColor;
	// Use this for initialization
	void Start () {
        inicialColor = this.GetComponent<MeshRenderer>().material.color;
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (Physics.Raycast(new Vector3(this.transform.position.x, -1.0f, this.transform.position.z), Vector3.up, out hit, Mathf.Infinity, mask))
        { 
             if (hit.transform.tag == "Player" || hit.transform.tag == "Monster" || hit.transform.tag == "Obstacles")          
                    this.GetComponent<MeshRenderer>().material.color = Color.red;

        }
        else
            this.GetComponent<MeshRenderer>().material.color = inicialColor;

        //this.transform.localEulerAngles = Vector3.zero;
	}
}
