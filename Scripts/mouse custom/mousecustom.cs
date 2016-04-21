using UnityEngine;
using System.Collections;

public class mousecustom : MonoBehaviour {

	// Use this for initialization
    public Texture2D cursorTexture;
	void Start () {

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
