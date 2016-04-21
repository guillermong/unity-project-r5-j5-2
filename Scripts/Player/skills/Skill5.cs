using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill5 : NetworkBehaviour
{

    public int skillID = 5;
    public float damage = 20.0f;
    public float cooldown = 1.0f;
    public float radius = 1;
    public int range = 8;
    public KeyCode KeySkill;

    public Sprite skillbarSprite;
    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;
    Camera cameraPlayer;

    public float castTime = 0.1f;
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

    public GameObject skill5Prefab;

    void Start()
    {
        KeySkill = KeyCode.Alpha5;

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
                if (GetComponent<StatsPlayer>().displayRangeSkillActive)
                    displayRange.GetComponent<rangetrack>().display();
                else
                    displayRange.GetComponent<rangetrack>().undisplay();
                activeTarget = true;
                Camera.main.GetComponent<CameraController>().SetShowArea(true, radius);
                Camera.main.GetComponent<CameraController>().isSkillActive = 1;
            }
            else if (activeTarget && (Input.GetMouseButtonDown(0) || (this.GetComponent<StatsPlayer>().smartCast && !click)))
            {
                click = true;
            }
            else if (activeTarget && click
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GetComponent<StatsPlayer>().maskSuelo)
                && GetComponent<StatsPlayer>().canskill == 0)
            {
                Destroy(displayRange);
                point = hit.point;
                CmdKarthus(hit.point);
                Camera.main.GetComponent<CameraController>().SetShowArea(false, 0);
                Camera.main.GetComponent<CameraController>().isSkillActive = 0;
                activeTarget = false;
                click = false;

            }
            else if (activeTarget && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.LeftShift))
            {
                Destroy(displayRange);
                Camera.main.GetComponent<CameraController>().SetShowArea(false, 0);
                activeTarget = false;
                click = false;
            }

        }
    }

    [Command]
    public void CmdKarthus(Vector3 _point)
    {
        if ( GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0)
        {
            point = _point;
            this.GetComponent<StatsPlayer>().skillActive = skillID;
            if(Pathfinding2.GetDistance2(
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(this.transform.position),
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(point)) > range)
            {
                this.GetComponent<Unit>().move2(point, range, CastSkill5);
                this.GetComponent<Unit>().RpcMoveClient(range, point,transform.position);
            }
            else
            {

                this.GetComponent<Unit>().move2(this.transform.position, range, CastSkill5);
                this.GetComponent<Unit>().RpcMoveClient(range, this.transform.position, transform.position);
            }                      
        }

    }

    [Server]
    public void CastSkill5()
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
        GameObject skill5Controller = (GameObject)Instantiate(skill5Prefab);
        skill5Controller.transform.position = new Vector3(this.point.x, skill5Controller.transform.position.y, this.point.z);

        skill5Controller.GetComponent<Skill5Karthus>().playerOwner = this.GetComponent<NetworkIdentity>().netId;
        skill5Controller.GetComponent<Skill5Karthus>().damage = damage;

        Destroy(skill5Controller, 0.5f);
        NetworkServer.Spawn(skill5Controller);


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
