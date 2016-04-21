using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class InBattleController : NetworkBehaviour
{


    public float timeBattle = 5;
    public float current_time1;
    public bool inbattle = false;

	// Use this for initialization
	void Start () 
    {
            current_time1 = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer) { 
            if (current_time1 > 0 && inbattle)
                {
                    current_time1 -= Time.deltaTime;
                    if (current_time1 <= 0)
                    {

                        inbattle = false;
                        RpcInBattle(false);
                        StartCoroutine("health_battle");
                    }

                }
        }
	
	}

    [Server]
    IEnumerator health_battle()
    {
        while (this.GetComponent<StatsPlayer>().current_hp < this.GetComponent<StatsPlayer>().max_hp && 
            !inbattle && 
            !this.GetComponent<StatsPlayer>().death)
        {
            this.GetComponent<StatsPlayer>().TakeHeal(this.GetComponent<StatsPlayer>().max_hp / 10);
            yield return new WaitForSeconds(0.5f);
        }

    }

    [Server]
    public void SetInBattle()
    {
        inbattle = true;
        RpcInBattle(true);
        current_time1 = timeBattle;
        
    }

    [ClientRpc]
    public void RpcInBattle(bool _inbattle)
    {
        inbattle = _inbattle;
        if (!_inbattle && GetComponent<StatsPlayer>().animatorPlayer.GetInteger("estado") == 3)
            GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 1);

    }
}
