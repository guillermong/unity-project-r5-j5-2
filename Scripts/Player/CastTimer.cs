using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class CastTimer : NetworkBehaviour
{

    public Vector3 point;
    public GameObject spell;

	// Use this for initialization
	void Start () {
        GetComponent<StatsPlayer>().parentBar.SetActive(false);
        GetComponent<StatsPlayer>().parentBarSkill.SetActive(false);
        
	}

    [Server]
    public IEnumerator castSkill(float _time, Action callback, Vector3 pointSkill)
    {
        if (!GetComponent<StatsPlayer>().isCasting) { 
            GetComponent<StatsPlayer>().isCasting = true;
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;
        

            float timeCast = _time - _time * (GetComponent<StatsPlayer>().dex / 150);
            float time = 0.0f;
            RpcInitCast(timeCast, pointSkill);

            while (GetComponent<StatsPlayer>().isCasting && time <= timeCast && !GetComponent<StatsPlayer>().death)
            {
                time += Time.deltaTime; 
                yield return null;             
            }

            GetComponent<StatsPlayer>().canskill--;
            GetComponent<StatsPlayer>().canWalk--;
            GetComponent<StatsPlayer>().canNornamAttack--;

            if (time >= timeCast && GetComponent<StatsPlayer>().isCasting) {
                GetComponent<StatsPlayer>().isCasting = false;
                GetComponent<StatsPlayer>().RpcSetisCasting(false);
                callback();
            }
        }
    }

    [ClientRpc]
    public void RpcInitCast(float _castTime, Vector3 pointSkill)
    {
        point = pointSkill;
        StartCoroutine(InitCast(_castTime));     
    }

    [Client]
    IEnumerator InitCast(float _castTime) {
        if (!isServer)
        {
            GetComponent<StatsPlayer>().isCasting = true;
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;
        }

        if (GetComponent<StatsPlayer>().skillActive == 3) 
        {
            spell = (GameObject)Instantiate(GetComponent<Skill3>().landPrefab, new Vector3( point.x,0.05f,point.z), GetComponent<Skill3>().landPrefab.transform.rotation);
            spell.transform.localScale *= GetComponent<Skill3>().radius;
        }

        GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 4);
        GetComponent<StatsPlayer>().parentBarSkill.SetActive(true);
        float time = 0.0f;
        while (GetComponent<StatsPlayer>().isCasting && time <= _castTime && !GetComponent<StatsPlayer>().death)
        {
            
            float result = time / _castTime;
            GetComponent<StatsPlayer>().bar_CastSkill.transform.localScale = new Vector3(Mathf.Clamp(result, 0f, 1f),
                                                                                        GetComponent<StatsPlayer>().bar_CastSkill.transform.localScale.y,
                                                                                        GetComponent<StatsPlayer>().bar_CastSkill.transform.localScale.z);
            time += Time.deltaTime;
            yield return null;
            //time += 0.1f;
            //yield return new WaitForSeconds(0.1f);
        }
        if (!isServer)
        {
            GetComponent<StatsPlayer>().canskill--;
            GetComponent<StatsPlayer>().canWalk--;
            GetComponent<StatsPlayer>().canNornamAttack--;
        }

        GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 1);
        GetComponent<StatsPlayer>().parentBarSkill.SetActive(false);
        Destroy(spell);
    }

}
