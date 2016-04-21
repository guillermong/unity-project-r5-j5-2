using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill6Safety : NetworkBehaviour
{

    public float time = 10.0f;
    public float count = 10.0f;
    public NetworkInstanceId playerOwner;


	// Use this for initialization
	void Start () {
        if(isServer)
            StartCoroutine(live());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [Server]
    IEnumerator live()
    {
        while (count > 0 && time > 0) 
        {
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
