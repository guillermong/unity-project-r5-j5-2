using UnityEngine;
using System.Collections;

public class timecount : MonoBehaviour {

    public int time;

	// Use this for initialization
	void Start () {

        StartCoroutine("TimeDown");
	}
	
	// Update is called once per frame
	void Update () {

	
	}

    IEnumerator TimeDown()
    {
        while(time > 0){
            this.GetComponent<GUIText>().text = time.ToString();
            yield return new WaitForSeconds(1.0f);
        }
    }
   
}
