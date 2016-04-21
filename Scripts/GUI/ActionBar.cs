using UnityEngine;
using System.Collections;

public class ActionBar : MonoBehaviour {

    public Texture2D actionBar;
    public Rect position;

    public int numberSkills = 7;
    public SkillSlot[] skill;

	// Use this for initialization
	void Start () {
        skill = new SkillSlot[numberSkills];

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        drawActionBar();
       
    }

    void drawActionBar()
    {
        GUI.DrawTexture(new Rect(Screen.width * position.x, Screen.height * position.y, Screen.width * position.width, Screen.height * position.height), actionBar);
    }
}
