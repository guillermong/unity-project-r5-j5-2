using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class Skill7 : NetworkBehaviour
{

    public int skillID = 7;
    public float cooldown = 20.0f;
    public KeyCode KeySkill;

    public Sprite skillbarSprite;
    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;

    void Start()
    {
        KeySkill = KeyCode.Alpha7;

    }



    // Update is called once per frame
    void LateUpdate()
    {

        if (isLocalPlayer && !GetComponent<StatsPlayer>().ONGUI)
        {
            if (Input.GetKeyDown(KeySkill))
            {
                CmdCloack();
            }
            
        }
    }

    [Command]
    public void CmdCloack()
    {
        if (GetComponent<StatsPlayer>().canskill == 0 && (cooldown_curring == 0 || GetComponent<BuffsDebuffsPlayer>().cloack))
        {
            if (GetComponent<BuffsDebuffsPlayer>().cloack)
                StartCoroutine("CoolDown");
            GetComponent<BuffsDebuffsPlayer>().SetCloack(!GetComponent<BuffsDebuffsPlayer>().cloack);
        }
    }

    

    [Server]
    public IEnumerator CoolDown()
    {

        cooldown_curring = cooldown;
        yield return new WaitForSeconds(cooldown);
        cooldown_curring = 0;
    }

    [Client]
    public void CoolDownHook(float _cooldown)
    {
        if (isLocalPlayer)
        {
            if (_cooldown > 0)
            {
                GameObject.Find("SKILLBAR").GetComponent<SkillBar>().StartCoroutine(GameObject.Find("SKILLBAR").GetComponent<SkillBar>().InitCooldown(skillID, _cooldown));
            }
        }

        cooldown_curring = _cooldown;
    }
}
