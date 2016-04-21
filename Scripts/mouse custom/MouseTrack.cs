using UnityEngine;
using System.Collections;

public class MouseTrack : MonoBehaviour {

    public LayerMask mask3TargetGround;
    RaycastHit hit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Camera.main) {
            Physics.Raycast( Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask3TargetGround);
            transform.position = hit.point;

        }
	}
}
