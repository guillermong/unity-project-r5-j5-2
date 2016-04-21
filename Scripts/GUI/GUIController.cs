using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    public GameObject Player;
    public GameObject button;
    public GameObject menu;
    public GameObject keybinding;
    public GameObject skillbar;

	// Use this for initialization
	void Start () {



	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowMenu()
    {
        if(menu.activeSelf){
            menu.SetActive(false);
            Player.GetComponent<StatsPlayer>().ONGUI = false;
        }
        else { 
            menu.SetActive(true);
            Player.GetComponent<StatsPlayer>().ONGUI = true;
        }
    }

    public void Showkeybinding()
    {
        if (keybinding.activeSelf){
            keybinding.SetActive(false);
            Player.GetComponent<StatsPlayer>().ONGUI = false;
        }
        else
        {
            keybinding.GetComponent<KeyBindingController>().updatebuttons();
            keybinding.GetComponent<KeyBindingController>().showkeys();
            keybinding.SetActive(true);
            Player.GetComponent<StatsPlayer>().ONGUI = true;
        }
            

    }

    public void Setplayer(GameObject _Player)
    {
        Player = _Player;
        button.SetActive(true);
        keybinding.GetComponent<KeyBindingController>().keysskill = new KeyCode[GameObject.Find("GUIplayer").GetComponent<GUIController>().keybinding.GetComponent<KeyBindingController>().keys.Length];
        keybinding.GetComponent<KeyBindingController>().keysskillaux = new KeyCode[GameObject.Find("GUIplayer").GetComponent<GUIController>().keybinding.GetComponent<KeyBindingController>().keys.Length];
        //skillbar.SetActive(true);
        skillbar.GetComponent<SkillBar>().SetSkillBar();

    }



}
