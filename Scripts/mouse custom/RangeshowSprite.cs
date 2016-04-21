using UnityEngine;
using System.Collections;

public class RangeshowSprite : MonoBehaviour {

    public LayerMask mask3TargetGround;
    RaycastHit hit;
    public Transform Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Camera.main)
        {
            Physics.Raycast(Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask3TargetGround);
            transform.position = Player.position;
            transform.LookAt(hit.point);

        }

	}
}
