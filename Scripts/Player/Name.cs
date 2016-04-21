using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Name : MonoBehaviour {

    public string namePlayer = "Anonimo";

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
               GetComponent<TextMesh>().text =  namePlayer;
	}

    void OnGUI()
    {

        namePlayer = GUI.TextField(new Rect(25, Screen.height-40,100,30), namePlayer);

    }
}
