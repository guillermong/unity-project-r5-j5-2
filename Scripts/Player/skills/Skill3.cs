using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill3 : NetworkBehaviour
{

    public int skillID = 3;
    public float cooldown = 1.5f;
    public float damage = 15;
    public int range = 3;
    public KeyCode KeySkill;

    public float radius = 2;
    public float castTime = 1.0f;
    public float stunTime = 3.0f;
    public GameObject landPrefab;
    public GameObject hitPrefab;
    public GameObject rangePrefab;
    public LayerMask mask;
    GameObject displayRange;

    public Sprite skillbarSprite;
    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;
    bool activeTarget = false;
    bool click = false;

    public Vector3 point;

    void Start()
    {
        KeySkill = KeyCode.Alpha3;

    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (isLocalPlayer && !GetComponent<StatsPlayer>().ONGUI)
        {
            RaycastHit hit;

            if (Input.GetKeyDown(KeySkill))
            {
                if (displayRange == null) { 
                    displayRange = (GameObject)Instantiate(rangePrefab);
                    displayRange.GetComponent<rangetrack>().player = this.gameObject;
                    displayRange.GetComponent<rangetrack>().range = range;
                }
                if (GetComponent<StatsPlayer>().displayRangeSkillActive)
                    displayRange.GetComponent<rangetrack>().display();
                else
                    displayRange.GetComponent<rangetrack>().undisplay();
                Camera.main.GetComponent<CameraController>().SetShowArea(true, radius);
                activeTarget = true;
            }
            else if (activeTarget && (Input.GetMouseButtonDown(0) || (this.GetComponent<StatsPlayer>().smartCast && !click)) )
            {
                click = true;
            }
            else if (activeTarget && click 
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, GetComponent<StatsPlayer>().maskSuelo)
                && GetComponent<StatsPlayer>().canskill == 0)
            {
                Destroy(displayRange);
                point = hit.point;
                CmdGolpeSuelo(hit.point);
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
    public void CmdGolpeSuelo(Vector3 _point)
    {
        if (//GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(_point).walkable &&
            GetComponent<StatsPlayer>().canskill == 0 &&
            cooldown_curring == 0)
        {
            point = _point;
            this.GetComponent<StatsPlayer>().skillActive = skillID;
            this.GetComponent<Unit>().move2(point, range, CastSkill3);
            this.GetComponent<Unit>().RpcMoveClient(range, point,transform.position);

        }
        
    }

    [Server]
    public void CastSkill3()
    {
        if (this.GetComponent<StatsPlayer>().skillActive == skillID
                    && GetComponent<StatsPlayer>().canskill == 0
                    && Pathfinding2.GetDistance2(
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(this.transform.position),
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(point)) <= range
                    && !GetComponent<NetworkPlayer>().walking
            && cooldown_curring == 0
            && !GetComponent<StatsPlayer>().isCasting)
        {
            StartCoroutine(GetComponent<CastTimer>().castSkill(castTime, Skill3Attack, point));

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere ( point, (0.5f*radius)/2);
    }

    [Server]
    public void Skill3Attack()
    {
        if (GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0){
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
        GetComponent<StatsPlayer>().canskill--;
        GetComponent<StatsPlayer>().canWalk--;
        GetComponent<StatsPlayer>().canNornamAttack--;
        StartCoroutine("CoolDown");
        Collider[] hitplayers = Physics.OverlapSphere(point, (0.5f * radius) / 2, mask);

        foreach (Collider players in hitplayers)
        {
            if ((players.gameObject.tag == "Player" || players.gameObject.tag == "OwnerPlayer") && players != this.GetComponent<CharacterController>())
            {

                players.transform.gameObject.GetComponent<BuffsDebuffsPlayer>().StartCoroutine("SetStun", stunTime);
                players.transform.gameObject.GetComponent<StatsPlayer>().TakeDamage(damage, this.GetComponent<NetworkIdentity>().netId);

            }
        }
        yield return null;
    }

    [Client]
    IEnumerator Skill3FinishClient()
    {
        GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 8);
        if (!isServer) { 
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;           
            yield return new WaitForSeconds(0.2f / this.GetComponent<StatsPlayer>().speedatk);
            GetComponent<StatsPlayer>().canskill--;
            GetComponent<StatsPlayer>().canWalk--;
            GetComponent<StatsPlayer>().canNornamAttack--; 
        }
        GameObject hit = Instantiate(hitPrefab, point, Quaternion.identity) as GameObject;
        hit.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Destroy(hit, 2);
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
