using UnityEngine;
using System.Collections;

public class rangetrack : MonoBehaviour {

    public GameObject player;
    public Vector3 offset;

    public int range;

	// Use this for initialization
	void Start () {
        offset = Vector3.zero;

        /*foreach (Transform child in transform)
            child.gameObject.SetActive(false);  */   

	}

	// Update is called once per frame
	void LateUpdate () {

        if (Input.GetKey(KeyCode.LeftShift))
            display();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            undisplay();

        this.transform.position = player.transform.position + offset;

        
	}

    public void display() {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        int i = 1;
        foreach (Transform child in transform)
        {
            if (i > range)
                break;

            child.gameObject.SetActive(true);
            i++;
        }   
    }

    public void undisplay()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

}
