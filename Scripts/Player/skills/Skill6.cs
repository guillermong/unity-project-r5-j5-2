using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill6 : NetworkBehaviour
{

    public int skillID = 6;
    public float cooldown = 5.0f;
    public int range = 4;
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

    public GameObject skill6Prefab;

    void Start()
    {
        KeySkill = KeyCode.Alpha6;

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
                CmdSafetywall(hit.point);
                Camera.main.GetComponent<CameraController>().isSkillActive = 0;
                activeTarget = false;
                click = false;

            }
            else if (activeTarget && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.LeftShift))
            {
                Destroy(displayRange);
                activeTarget = false;
                click = false;
            }

        }
    }

    [Command]
    public void CmdSafetywall(Vector3 _point)
    {
        if (GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0 && GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(_point).walkable)
        {
            point = _point;
            this.GetComponent<StatsPlayer>().skillActive = skillID;
            if (Pathfinding2.GetDistance2(
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(this.transform.position),
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(point)) > range)
            {
                this.GetComponent<Unit>().move2(point, range, CastSkill6);
                this.GetComponent<Unit>().RpcMoveClient(range, point, transform.position);
            }
            else
            {

                this.GetComponent<Unit>().move2(this.transform.position, range, CastSkill6);
                this.GetComponent<Unit>().RpcMoveClient(range, this.transform.position, transform.position);
            }
        }

    }

    [Server]
    public void CastSkill6()
    {
        if (this.GetComponent<StatsPlayer>().skillActive == skillID
                    && GetComponent<StatsPlayer>().canskill == 0
                    && !GetComponent<NetworkPlayer>().walking
            && cooldown_curring == 0
            && !GetComponent<StatsPlayer>().isCasting)
        {
            StartCoroutine(GetComponent<CastTimer>().castSkill(castTime, Skill6Attack, point));
        }
    }


    [Server]
    public void Skill6Attack()
    {
        if (GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0)
        {
            StartCoroutine(Skill6Finish());
            RpcSkill6Finish();
        }
    }

    [ClientRpc]
    public void RpcSkill6Finish()
    {
        StartCoroutine(Skill6FinishClient());
    }

    [Server]
    IEnumerator Skill6Finish()
    {
        GetComponent<StatsPlayer>().canskill++;
        GetComponent<StatsPlayer>().canWalk++;
        GetComponent<StatsPlayer>().canNornamAttack++;
        yield return new WaitForSeconds(0.2f / this.GetComponent<StatsPlayer>().speedatk);

        //TIRAR SKILL
        Vector3 pointsafety = GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(point).worldPosition;
        
        if (!Physics.Raycast(new Vector3(pointsafety.x, -10f, pointsafety.z), Vector3.up, out hit, Mathf.Infinity, mask))
        {
            GameObject skill6Controller = (GameObject)Instantiate(skill6Prefab);
            skill6Controller.transform.position = new Vector3(pointsafety.x, skill6Controller.transform.position.y, pointsafety.z);
            skill6Controller.GetComponent<Skill6Safety>().playerOwner = this.GetComponent<NetworkIdentity>().netId;

            //Destroy(skill5Controller, 0.5f);
            NetworkServer.Spawn(skill6Controller);
            StartCoroutine("CoolDown");
        }

        GetComponent<StatsPlayer>().canskill--;
        GetComponent<StatsPlayer>().canWalk--;
        GetComponent<StatsPlayer>().canNornamAttack--;
        



        yield return null;
    }

    [Client]
    IEnumerator Skill6FinishClient()
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
