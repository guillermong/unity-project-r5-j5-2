using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BuffsDebuffsPlayer : NetworkBehaviour
{
    //stun
    public bool stun = false;
    public bool antistun = false;
    public float timeStun = 0.0f;
    public GameObject stunG;
    //stuck
    public bool stuck = false; 
    public GameObject stuckG;
    //safetywall
    //public bool SafetyWall = false;

    //cloack
    public bool cloack = false;
    public SpriteRenderer head;
    public SpriteRenderer body;
    public TextMesh name;
    public GameObject hp1;
    public GameObject hp2;
    public GameObject hp3;

	// Use this for initialization
	void Start () {
        stunG.SetActive(false);
        stuckG.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    //ARREGLAR ESTA COMO PICO PROGRAMADO
    [Server]
    public IEnumerator SetStun(float _timeStun_)
    {
        if (!antistun && !stun)
        {
            timeStun += _timeStun_;
            stun = true;
            RpcStun(stun,_timeStun_);
            this.GetComponent<StatsPlayer>().canWalk++;
            this.GetComponent<StatsPlayer>().canskill++;
            this.GetComponent<StatsPlayer>().canNornamAttack++;

            float _timeStun = 0.0f;
            while (_timeStun <= timeStun && stun && !GetComponent<StatsPlayer>().death)
            {
                _timeStun += Time.deltaTime;
                yield return null;
            }
            timeStun = 0.0f;
            stun = false;
            RpcStun(stun, _timeStun_);
            this.GetComponent<StatsPlayer>().canWalk--;
            this.GetComponent<StatsPlayer>().canskill--;
            this.GetComponent<StatsPlayer>().canNornamAttack--;
        }else if(stun)
            timeStun += _timeStun_;

    }

    [ClientRpc]
    public void RpcStun(bool _stun,float time)
    {
        stun = _stun;
        timeStun += time;
            if (_stun)
            {
                if (!isServer)
                {
                    this.GetComponent<StatsPlayer>().canWalk++;
                    this.GetComponent<StatsPlayer>().canskill++;
                    this.GetComponent<StatsPlayer>().canNornamAttack++;
                }
                stunG.SetActive(true);
            }
            else {
                if (!isServer)
                {
                    this.GetComponent<StatsPlayer>().canWalk--;
                    this.GetComponent<StatsPlayer>().canskill--;
                    this.GetComponent<StatsPlayer>().canNornamAttack--;
                    
                }
                stunG.SetActive(false); 
            }
     }


    [Server]
    public IEnumerator SetStuck(float _timeStuck_)
    {
        stuck = true;
        float time = 0;
        RpcSetStuckClient(stuck,_timeStuck_);
        this.GetComponent<StatsPlayer>().canWalk++;
        while (stuck && time < _timeStuck_ && !GetComponent<StatsPlayer>().death)
        {
            time += Time.deltaTime;
            yield return null;
        }
        stuck = false;
        this.GetComponent<StatsPlayer>().canWalk--;
        RpcSetStuckClient(stuck, _timeStuck_);

    }

    [ClientRpc]
    public void RpcSetStuckClient(bool _stuck, float time)
    {
        stuck = _stuck;
        timeStun += time;
        if (_stuck)
        {
            if (!isServer)
            {
                this.GetComponent<StatsPlayer>().canWalk++;
            }
            stuckG.SetActive(true);
        }
        else
        {
            if (!isServer)
            {
                this.GetComponent<StatsPlayer>().canWalk--;
            }
            stuckG.SetActive(false);
        }

    }

    [Server]
    public void SetCloack(bool _cloack)
    {
        cloack = _cloack;
        if (!cloack)
            this.gameObject.layer = LayerMask.NameToLayer("players");
        else 
            this.gameObject.layer = LayerMask.NameToLayer("cloack");
        RpcSetCloack(cloack);
        if (cloack) StartCoroutine(timecloack());
    }

    [ClientRpc]
    public void RpcSetCloack(bool _cloack)
    {
        cloack = _cloack;
        if (cloack) 
        { 
            StartCoroutine(cloacking());
            this.gameObject.layer = LayerMask.NameToLayer("cloack");
            
        }
        else
        {
            if(isLocalPlayer)
                this.gameObject.layer = LayerMask.NameToLayer("ownerplayer");
            else
                this.gameObject.layer = LayerMask.NameToLayer("players");
            Color tmp = body.color;
            tmp.a = 255;
            body.color = tmp;
            head.color = tmp;
        }
    }

    IEnumerator timecloack()
    {
        yield return new WaitForSeconds(10);
        if (cloack)
            SetCloack(false);
    }

    [Client]
    IEnumerator cloacking() {
        while (body.color.a > 0 && cloack)
        {
            Color tmp = body.color;
            tmp.a -= 15;
            body.color = tmp;
            head.color = tmp;
            yield return new WaitForSeconds(0.1f);
        }
        
    }


}
