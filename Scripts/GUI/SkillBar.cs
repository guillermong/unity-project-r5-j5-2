using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour {

    public Button[] skillSlots;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void SetSkillBar()
    {
        this.gameObject.SetActive(true);
        skillSlots[0].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill1.skillbarSprite;
        skillSlots[1].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill2.skillbarSprite;
        skillSlots[2].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill3.skillbarSprite;
        skillSlots[3].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill4.skillbarSprite;
        skillSlots[4].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill5.skillbarSprite;
        skillSlots[5].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill6.skillbarSprite;
        skillSlots[6].GetComponent<Image>().sprite = GetComponentInParent<GUIController>().Player.GetComponent<SkillController>().skill7.skillbarSprite;


    }

    public IEnumerator InitCooldown(int id, float time)
    {
        float timeaux = time;
        while (time > 0)
        {

            skillSlots[id - 1].transform.FindChild("image").GetComponent<Image>().fillAmount = time / timeaux;
            skillSlots[id - 1].GetComponentInChildren<Text>().text = ((int)time).ToString();

            time -= Time.deltaTime;
            yield return null;
        }
        skillSlots[id - 1].transform.FindChild("image").GetComponent<Image>().fillAmount = 0;
        skillSlots[id - 1].GetComponentInChildren<Text>().text = "";


    }

}
