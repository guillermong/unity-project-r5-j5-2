using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class StatsPlayer : NetworkBehaviour
{
    
    //STATS PLAYER
    [SyncVar(hook = "SyncPositionHook")]
    public Vector3 positionPlayer;
    [SyncVar(hook = "SyncRotationHook")]
    public Quaternion rotationPlayer;
    [SyncVar]
    public string namePlayer = "Anonimo";
    [SyncVar(hook = "SetHealthBar")]
    public float current_hp;
    [SyncVar]
    public float max_hp = 100.0f;
    
    public float current_cast1;
    public float current_cast2;

    [SyncVar]
    public float recovery_hp = 3.0f;
    [SyncVar]
    public float dex = 1;

    [SyncVar]
    public float speed = 5;
    public float speedatk = 1.0f;
    public bool death = false;

    //COMPONENTES GAME
    public int timedie = 30;
    public bool smartCast = false;
    public bool displayRangeSkillActive = false;

    public LayerMask mask;
    public LayerMask maskSuelo;
    public LayerMask maskhit;

    [SyncVar(hook = "skillactivehook")]
    public int skillActive = 0;

    


    public bool isCasting = false;
    public bool antiStopCast = false;
    public int canWalk = 0;
    public int canskill = 0;
    public int canNornamAttack = 0;

    public GameObject bar_Hp;
    public GameObject bar_Cast;
    public GameObject parentBar;
    public GameObject bar_CastSkill;
    public GameObject parentBarSkill;
    public GameObject Damageprefab;
    public GameObject textPrefab;
    public Animator animatorPlayer;

    bool setname = true;
    public bool ONGUI = false;

    void OnGUI()
    {
        //namePlayer = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), namePlayer);
    }

	void Start () {
        if (isServer)
        {
            InvokeRepeating("CmdRecovery", 1f, 1f);
        }
	}
	
    void Update()
    {
      
        if (isLocalPlayer )
        {
            if (GameObject.Find("CHAT").GetComponent<ChatGui>().isActiveAndEnabled && setname) { 
                namePlayer = GameObject.Find("CHAT").GetComponent<ChatGui>().UserName;
                CmdChangeName(namePlayer);
                setname = false;
            }
        }
        this.GetComponentInChildren<TextMesh>().text= namePlayer;

        
    }

    [Server]
    public void TakeDamage(float damage, NetworkInstanceId playerEnemyID)
    {

        if (current_hp > 0) {
            RaycastHit hit;
            if (!Physics.Raycast(new Vector3(transform.position.x, -10f, transform.position.z), Vector3.up, out hit, Mathf.Infinity, maskhit))
            { 
                current_hp -= damage;
                this.GetComponent<InBattleController>().SetInBattle();
                NetworkServer.FindLocalObject(playerEnemyID).GetComponent<InBattleController>().SetInBattle();

                //this.GetComponent<BuffsDebuffsPlayer>().cloack = false;
                //NetworkServer.FindLocalObject(playerEnemyID).GetComponent<BuffsDebuffsPlayer>().cloack = false;

                RpcDamageDisplay(damage);

                if(isCasting && !antiStopCast)
                    isCasting = false;
            }
            else
            {
                hit.transform.gameObject.GetComponentInParent<Skill6Safety>().count--;
            }
        }

        
        if (current_hp <= 0)
        {
            StartCoroutine("die");          
            RpcWhokill(namePlayer, NetworkServer.FindLocalObject(playerEnemyID).GetComponent<StatsPlayer>().namePlayer);
            
        }
        else
        {           
            StopCoroutine("stuckspeed");
            StartCoroutine("stuckspeed");
        }

    }

    [Server]
    public void TakeHeal(float heal)
    {

        if (current_hp > 0)
        {
            current_hp += heal;
            if (current_hp > max_hp) current_hp = max_hp;
            RpcDamageDisplay(-heal);
        }

    }

    [Server]
    void CmdRecovery()
    {
        if(current_hp < max_hp)
            TakeHeal(recovery_hp);
    }

    [Server]
    IEnumerator die()
    {
        skillActive = 0;        
        canskill = 9999;
        canWalk = 9999;
        canNornamAttack = 9999;        
        if (GetComponent<NetworkPlayer>().walking)
        {
            GetComponent<Unit>().StopCoroutine("FollowPath");
            GetComponent<NetworkPlayer>().walking = false;
        }
        death = true;
        RpcDeathController(true);

        yield return new WaitForSeconds(timedie);
        if (death)
        {
            death = false;
            RpcDeathController(death);
            canskill = 0;
            canWalk = 0;
            canNornamAttack = 0;
            current_hp = max_hp;
            positionPlayer = GameObject.Find("SpawnPlayer").transform.GetChild(Random.Range(1, GameObject.Find("SpawnPlayer").transform.childCount)).position;
        }
    }

    [ClientRpc]
    public void RpcDeathController(bool _death)
    {
        death = _death;
        if (_death)
        {
            if (!isServer) { 
            canskill = 9999;
            canWalk = 9999;
            canNornamAttack = 9999;
            
            }
            if (GetComponent<NetworkPlayer>().walking)
            {
                GetComponent<Unit>().StopCoroutine("FollowPath");
                GetComponent<NetworkPlayer>().walking = false;
            }

            animatorPlayer.SetInteger("estado", 7);
            this.gameObject.layer = LayerMask.NameToLayer("death");
            if (isLocalPlayer)
            {

                GameObject aux = Instantiate(textPrefab) as GameObject;
                aux.name = "timedie";
                aux.GetComponent<GUIText>().color = Color.white;
                aux.GetComponent<GUIText>().fontSize = 30;
                aux.GetComponent<GUIText>().alignment = TextAlignment.Center;
                aux.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
                aux.transform.position = new Vector3(0.5f, 0.75f, 0);
                StartCoroutine("TimeDown", aux);
            }
        }
        else
        {
            if (!isServer)
            {
                canskill = 0;
                canWalk = 0;
                canNornamAttack = 0;
            }
            if (isLocalPlayer)
                this.gameObject.layer = LayerMask.NameToLayer("ownerplayer");
            else
                this.gameObject.layer = LayerMask.NameToLayer("players");
            StopCoroutine("TimeDown");
            Destroy(GameObject.Find("timedie"));
            animatorPlayer.SetInteger("estado", 1);
        }

    }

    [ClientRpc]
    public void RpcWhokill(string name1, string name2)
    {
        if (GameObject.Find("kill"))
            Destroy(GameObject.Find("kill"));

        GameObject aux = Instantiate(textPrefab) as GameObject;
        aux.name = "kill";
        aux.GetComponent<GUIText>().color = Color.white;
        aux.GetComponent<GUIText>().fontSize = 40;
        aux.GetComponent<GUIText>().alignment = TextAlignment.Center;
        aux.GetComponent<GUIText>().anchor = TextAnchor.MiddleCenter;
        aux.transform.position = new Vector3(0.5f, 0.85f, 0);
        aux.GetComponent<GUIText>().text = "<color=yellow>" + name1 + "</color> owned <color=red>" + name2 + "</color>";
        Destroy(aux, 2);

    }



    [Server]
    IEnumerator stuckspeed()
    {
        speed = 0;
        yield return new WaitForSeconds(0.2f);
        speed = 5;

    }

    [Command]
    public void CmdChangeName(string _name)
    {
        namePlayer = _name;
    }

    [Command]
    public void CmdSkillActive(int SkillID)
    {
        if(canskill == 0)
            skillActive = SkillID;
    }
    
    [Command]
    public void CmdSyncRotation( Quaternion rotation)
    {
        rotationPlayer = rotation;
    }

    [Client]
    public void SyncRotationHook(Quaternion rotation) 
    {
        if (!isLocalPlayer)
        {
            this.transform.rotation = rotation;
            this.GetComponent<Unit>().rotationPlayer = rotation;
            //rotationPlayer = rotation;
        }       
    }

    
    [Client]
    public void SyncPositionHook(Vector3 position) 
    {
        this.transform.position = position;
        positionPlayer = position;
    }

    [Client]
    public void skillactivehook( int _skillactive)
    {
        if (_skillactive == 1)
        {
            animatorPlayer.SetInteger("estado", 4);
        }

        skillActive = _skillactive;
    }

    [Client]
    public void SetHealthBar(float _current_hp)
    {
        float myHealth = _current_hp / max_hp;
        bar_Hp.transform.localScale = new Vector3(Mathf.Clamp(myHealth, 0f, 1f), bar_Hp.transform.localScale.y, bar_Hp.transform.localScale.z);
        current_hp = _current_hp;
    }

    


    [Client]
    public void SetcastBar(Vector3 result)
    {
        bar_Cast.transform.localScale = result;
    }


    [Client]
    public void InitDamagePrefab(float damage)
    {

        GameObject aux = Instantiate(Damageprefab) as GameObject;
        Transform auxtransform = aux.transform;
        aux.transform.SetParent(this.transform);
        aux.transform.localPosition = Damageprefab.transform.localPosition;
        if (damage >= 0)
        {
            if(isLocalPlayer)
                aux.GetComponent<TextMesh>().color = Color.red;
            else
                aux.GetComponent<TextMesh>().color = Color.white;
        }
        else
        {
            aux.GetComponent<TextMesh>().color = Color.green;
            damage = Mathf.Abs(damage);
        }
            

        aux.GetComponent<TextMesh>().text = damage.ToString();

        aux.GetComponent<TextMesh>().fontSize += (int)((damage / max_hp) * 50); 
        aux.GetComponent<Animator>().SetTrigger("damage");
        Destroy(aux, 0.6f);
    }

    [ClientRpc]
    public void RpcSetAnimationEstado(int estado123)
    {
        animatorPlayer.SetInteger("estado", estado123);
    }
    
    [ClientRpc]
    public void RpcSetisCasting(bool _isCasting)
    {
        if(!isServer)
            isCasting = _isCasting;
    }

    [ClientRpc]
    public void RpcDamageDisplay(float damage)
    {

        InitDamagePrefab(damage);
        if (damage > 0)
        {
            animatorPlayer.SetInteger("estado", 3);
            animatorPlayer.Play("MONKDAMAGE");
        }   
    }




    [Client]
    IEnumerator TimeDown(GameObject _textprefab)
    {
        int time = timedie;
        while (time >= 0)
        {
            if (time < 5) 
            { 
                _textprefab.GetComponent<GUIText>().color = Color.red;
                _textprefab.GetComponent<GUIText>().fontSize = 40;
            }
            _textprefab.GetComponent<GUIText>().text = time.ToString();
            yield return new WaitForSeconds(1.0f);
            time--;
        }
        Destroy(_textprefab);

    }


}
