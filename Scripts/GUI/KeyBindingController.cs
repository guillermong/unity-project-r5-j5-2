using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeyBindingController : MonoBehaviour {

    public Button[] keys;
    public KeyCode[] keysskill;
    public KeyCode[] keysskillaux;
    private KeyCode[] desiredKeys = { 
                                        KeyCode.A, 
                                        KeyCode.B, 
                                        KeyCode.C,
                                        KeyCode.D,
                                        KeyCode.E,
                                        KeyCode.F,
                                        KeyCode.G,
                                        KeyCode.H,
                                        KeyCode.I,
                                        KeyCode.J,
                                        KeyCode.K,
                                        KeyCode.L,
                                        KeyCode.M,
                                        KeyCode.N,
                                        KeyCode.O,
                                        KeyCode.P,
                                        KeyCode.Q,
                                        KeyCode.R,
                                        KeyCode.S,
                                        KeyCode.T,
                                        KeyCode.U,
                                        KeyCode.V,
                                        KeyCode.W,
                                        KeyCode.X,
                                        KeyCode.Y,
                                        KeyCode.Z,
                                        KeyCode.Alpha0,
                                        KeyCode.Alpha1,
                                        KeyCode.Alpha2,
                                        KeyCode.Alpha3,
                                        KeyCode.Alpha4,
                                        KeyCode.Alpha5,
                                        KeyCode.Alpha6,
                                        KeyCode.Alpha7,
                                        KeyCode.Alpha8,
                                        KeyCode.Alpha9,
                                        KeyCode.Tab,
                                        KeyCode.Space,
                                    };



    public KeyCode HasALetterBeenPressed()
    {
        foreach (KeyCode keyToCheck in desiredKeys)
        {
            if (Input.GetKeyDown(keyToCheck)) 
                return keyToCheck;
        }
        return KeyCode.RightShift;
    }

   /* public Dictionary<KeyCode, Button> dictionary = new Dictionary<KeyCode, Button> { 

   { (int)KeyCode.Alpha1, 0 }, 
   { (int)KeyCode.Alpha2, 0 }, 
   { (int)KeyCode.Alpha3, 0 } 
    };*/


    

    public void ChangeKeyButton(int id)
    {
        StopCoroutine("waitforkey");
        StartCoroutine("waitforkey",id);

    }

    //AQUI SE HACE LOS CAMBIOS Y LAS VERIFICACIONES
    IEnumerator waitforkey(int id)
    {
        keys[id - 1].GetComponent<Image>().color = Color.yellow;
        GUI.FocusControl("MyTextField");
        while(!Input.anyKeyDown)
            yield return null;
        KeyCode auxkey = HasALetterBeenPressed();
        
        
        if (auxkey != KeyCode.RightShift) {
            int index = 0;
            foreach (KeyCode x in keysskill)
            {
                if (x == auxkey)
                {
                    keys[index].GetComponentInChildren<Text>().text = "" + keysskill[id - 1];
                    keysskill[index] = keysskill[id - 1];
                    if (keysskill[index] ==  keysskillaux[index])
                        keys[index].GetComponent<Image>().color = Color.white;
                    else
                        keys[index].GetComponent<Image>().color = Color.green;
                }
                index++;
            }
            keys[id - 1].GetComponentInChildren<Text>().text = "" + auxkey;
            keysskill[id - 1] = auxkey;
        }
        if (keysskill[id - 1] == keysskillaux[id - 1])
            keys[id - 1].GetComponent<Image>().color = Color.white;
        else
            keys[id - 1].GetComponent<Image>().color = Color.green;
        GUI.FocusControl("");
    }

    public void updatebuttons()
    {

        keysskill[0] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove1;
        keysskill[1] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove2;
        keysskill[2] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove3;
        keysskill[3] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove4;
        keysskill[4] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill1.KeySkill;
        keysskill[5] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill2.KeySkill;
        keysskill[6] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill3.KeySkill;
        keysskill[7] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill4.KeySkill;
        keysskill[8] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill5.KeySkill;
        keysskill[9] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill6.KeySkill;
        keysskill[10] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill7.KeySkill;

        keysskillaux[0] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove1;
        keysskillaux[1] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove2;
        keysskillaux[2] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove3;
        keysskillaux[3] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove4;
        keysskillaux[4] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill1.KeySkill;
        keysskillaux[5] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill2.KeySkill;
        keysskillaux[6] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill3.KeySkill;
        keysskillaux[7] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill4.KeySkill;
        keysskillaux[8] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill5.KeySkill;
        keysskillaux[9] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill6.KeySkill;
        keysskillaux[10] = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill7.KeySkill;

    }

    public void showkeys()
    {

        keys[0].GetComponentInChildren<Text>().text = "" + keysskill[0];
        keys[1].GetComponentInChildren<Text>().text = "" + keysskill[1];
        keys[2].GetComponentInChildren<Text>().text = "" + keysskill[2];
        keys[3].GetComponentInChildren<Text>().text = "" + keysskill[3];
        keys[4].GetComponentInChildren<Text>().text = "" + keysskill[4];
        keys[5].GetComponentInChildren<Text>().text = "" + keysskill[5];
        keys[6].GetComponentInChildren<Text>().text = "" + keysskill[6];
        keys[7].GetComponentInChildren<Text>().text = "" + keysskill[7];
        keys[8].GetComponentInChildren<Text>().text = "" + keysskill[8];
        keys[9].GetComponentInChildren<Text>().text = "" + keysskill[9];
        keys[10].GetComponentInChildren<Text>().text = "" + keysskill[10];


    }

    public void Applykeybinding()
    {

        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove1 = keysskill[0];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove2 = keysskill[1];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove3 = keysskill[2];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().move2.KeyMove4 = keysskill[3];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill1.KeySkill = keysskill[4];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill2.KeySkill = keysskill[5];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill3.KeySkill = keysskill[6];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill4.KeySkill = keysskill[7];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill5.KeySkill = keysskill[8];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill6.KeySkill = keysskill[9];
        GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill7.KeySkill = keysskill[10];


    }


}
