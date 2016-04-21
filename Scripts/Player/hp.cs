using UnityEngine;
using System.Collections;

public class hp : MonoBehaviour {


	void Start () {

	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation* Vector3.back, Camera.main.transform.rotation* Vector3.down  );
            //transform.LookAt(Camera.main.transform.position);
            //transform.Rotate(new Vector3(0, 180, 0));
        }
	}


}
