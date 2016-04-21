using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill4 : NetworkBehaviour
{ 

    public int skillID = 4;
    public float damage = 10.0f;
    public float cooldown = 8.0f;
    public int range = 7;
    public KeyCode KeySkill;

    public Sprite skillbarSprite;
    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;
    Camera cameraPlayer;

    public float castTime = 0.2f;
    public bool skillactive = false;
    RaycastHit hit;
    public LayerMask mask;
    GameObject playerattacked;

    private Vector3 Target;
    bool activeTarget = false;
    bool click = false;

    public GameObject rangePrefab;
    GameObject displayRange;
    Vector3 point;

    public GameObject skill4Prefab;
    public GameObject skill4Controller;
    public GameObject skill4Range;
    public GameObject skill4Rangeshow;
    private GameObject target2;

    void Start()
    {
        target2 = GameObject.Find("Target2");
        KeySkill = KeyCode.Alpha4;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (isLocalPlayer && !GetComponent<StatsPlayer>().ONGUI)
        {
            RaycastHit hit;

            if (Input.GetKeyDown(KeySkill))
            {
                if (displayRange == null)
                {
                    displayRange = (GameObject)Instantiate(rangePrefab);
                    displayRange.GetComponent<rangetrack>().player = this.gameObject;
                    displayRange.GetComponent<rangetrack>().range = range;
                }
                if (skill4Rangeshow == null) { 
                    skill4Rangeshow = (GameObject)Instantiate(skill4Range);
                    skill4Rangeshow.GetComponent<RangeshowSprite>().Player = this.transform;
                }
                if (GetComponent<StatsPlayer>().displayRangeSkillActive)
                    displayRange.GetComponent<rangetrack>().display();
                else
                    displayRange.GetComponent<rangetrack>().undisplay();
                activeTarget = true;
                Camera.main.GetComponent<CameraController>().isSkillActive = 1;
                target2.SetActive(false);
            }
            else if (activeTarget && ( Input.GetMouseButtonDown(0) || (this.GetComponent<StatsPlayer>().smartCast && !click) ) )
            {
                click = true;
            }
            else if (activeTarget && click
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GetComponent<StatsPlayer>().maskSuelo)
                && GetComponent<StatsPlayer>().canskill == 0)
            {
                Destroy(displayRange);
                Destroy(skill4Rangeshow);
                point = hit.point;
                CmdLux(hit.point);
                Camera.main.GetComponent<CameraController>().isSkillActive = 0;
                activeTarget = false;
                click = false;
                target2.SetActive(true);

            }
            else if (activeTarget && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.LeftShift))
            {
                Destroy(displayRange);
                Destroy(skill4Rangeshow);
                activeTarget = false;
                click = false;
                target2.SetActive(true);
            }

        }
    }

    [Command]
    public void CmdLux(Vector3 _point)
    {
        if ( GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0)
        {
            point = _point;
            this.GetComponent<StatsPlayer>().skillActive = skillID;
            this.GetComponent<Unit>().move2(this.transform.position, range, CastSkill4);
            this.GetComponent<Unit>().RpcMoveClient(range, this.transform.position,transform.position);

        }

    }

    [Server]
    public void CastSkill4()
    {
        if (this.GetComponent<StatsPlayer>().skillActive == skillID
                    && GetComponent<StatsPlayer>().canskill == 0              
                    && !GetComponent<NetworkPlayer>().walking
            && cooldown_curring == 0
            && !GetComponent<StatsPlayer>().isCasting)
        {
            StartCoroutine(GetComponent<CastTimer>().castSkill(castTime, Skill3Attack, point));
        }
    }


    [Server]
    public void Skill3Attack()
    {
        if (GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0)
        {
            StartCoroutine(Skill3Finish());
            RpcSkill3Finish();
        }
    }

    [ClientRpc]
    public void RpcSkill3Finish()
    {
        StartCoroutine(Skill3FinishClient());
    }

    [Server]
    IEnumerator Skill3Finish()
    {
        GetComponent<StatsPlayer>().canskill++;
        GetComponent<StatsPlayer>().canWalk++;
        GetComponent<StatsPlayer>().canNornamAttack++;
        yield return new WaitForSeconds(0.2f / this.GetComponent<StatsPlayer>().speedatk);

        //TIRAR SKILL
        skill4Controller = (GameObject)Instantiate(skill4Prefab);
        skill4Controller.transform.position = new Vector3(this.transform.position.x, skill4Controller.transform.position.y, this.transform.position.z);
        skill4Controller.GetComponent<Skill4Lux>().velocity =  (point- transform.position).normalized;
        skill4Controller.GetComponent<Skill4Lux>().playerOwner = this.GetComponent<NetworkIdentity>().netId;
        skill4Controller.GetComponent<Skill4Lux>().damage = damage;

        Destroy(skill4Controller, 0.5f);
        NetworkServer.Spawn(skill4Controller);


        GetComponent<StatsPlayer>().canskill--;
        GetComponent<StatsPlayer>().canWalk--;
        GetComponent<StatsPlayer>().canNornamAttack--;
        StartCoroutine("CoolDown");
        


        yield return null;
    }

    [Client]
    IEnumerator Skill3FinishClient()
    {
        GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 4);
        if (!isServer)
        {
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;
            yield return new WaitForSeconds(0.2f / this.GetComponent<StatsPlayer>().speedatk);
            GetComponent<StatsPlayer>().canskill--;
            GetComponent<StatsPlayer>().canWalk--;
            GetComponent<StatsPlayer>().canNornamAttack--;
        }

        //TIRAR SKILL

        yield return null;
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