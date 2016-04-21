using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill2 : NetworkBehaviour
{

    public int skillID = 2;
    public float cooldown = 1.0f;
    public float damage = 5;
    public int range = 6;
    public KeyCode KeySkill;
    public Sprite skillbarSprite;

    public float speedSnap = 30;
    public GameObject rangePrefab;

    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;

    bool activeTarget = false;
    bool click = false;
    public GameObject displayRange;
    public Vector3 point;

    void Start()
    {
        KeySkill = KeyCode.Alpha2;

    }
	// Update is called once per frame
	void LateUpdate () {

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
                else displayRange.GetComponent<rangetrack>().undisplay();

                Camera.main.GetComponent<CameraController>().isSkillActive = 1;
                activeTarget = true;
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
                CmdSnapSkill(hit.point);
                Camera.main.GetComponent<CameraController>().isSkillActive = 0;
                activeTarget = false;
                click = false;

            }
            else if (activeTarget && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.LeftShift))
            {
                Destroy(displayRange);
                Camera.main.GetComponent<CameraController>().isSkillActive = 0;
                activeTarget = false;
                click = false;
            }

        }
	}


    [Command]
    public void CmdSnapSkill(Vector3 _point)
    {
        
        if (GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(_point).walkable 
            && GetComponent<StatsPlayer>().canskill == 0
            && cooldown_curring == 0)
        {
            point = _point;
            this.GetComponent<StatsPlayer>().skillActive = skillID;
            this.GetComponent<Unit>().move2(point, range, snapSkill);
            this.GetComponent<Unit>().RpcMoveClient(range, point,transform.position);

        }

    }

    [Server]
    public void snapSkill()
    {
        if (this.GetComponent<StatsPlayer>().skillActive == skillID
                    && GetComponent<StatsPlayer>().canskill == 0
                    && GetComponent<StatsPlayer>().canWalk == 0
                    && Pathfinding2.GetDistance2(
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(this.transform.position),
                                    GameObject.Find("A*").GetComponent<Grid>().NodeFromWorldPoint(point)) <= range
                    && !GetComponent<NetworkPlayer>().walking)
        {
            PathRequestMaganer2.RequestPath(transform.position, point, snap);
            RpcMovesnap();

        }

    }

    [ClientRpc]
    public void RpcMovesnap()
    {
        PathRequestMaganer2.RequestPath(transform.position, point, snapclient);
    }

    [Server]
    public void snap(Vector3[] path, bool succes)
    {
        if (succes && isServer )
        {

            StartCoroutine(CoolDown());
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;
            if (path.Length > 0) 
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath", path);
            }
            else
            {
                finishSnap();
            }           
        }

    }

    [Client]
    public void snapclient(Vector3[] path, bool succes)
    {
        if (succes && !isServer)
        {
            GetComponent<StatsPlayer>().canskill++;
            GetComponent<StatsPlayer>().canWalk++;
            GetComponent<StatsPlayer>().canNornamAttack++;
            if (path.Length > 0)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath", path);
            }
            else
            {
                finishSnap();
            }
        }
    }


    IEnumerator FollowPath(Vector3[] path)
    {
        int targetIndex = 0;
        if (path.Length != 0)
        {
            Vector3 currentWaypoint = path[0];
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {                  
                        //yield return new WaitForFixedUpdate();
                        finishSnap();
                        break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                if (GetComponent<StatsPlayer>().death || GetComponent<StatsPlayer>().canWalk > 1)
                {         
                    finishSnap();
                    yield break;
                }
                GetComponent<Unit>().rotationPlayer = Quaternion.LookRotation(currentWaypoint - transform.position);
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speedSnap * Time.deltaTime);
                yield return null;
            }
        }
    }
 
    public void finishSnap()
    {
        this.GetComponent<StatsPlayer>().speed = 5;
        GetComponent<StatsPlayer>().canskill--;
        GetComponent<StatsPlayer>().canWalk--;
        GetComponent<StatsPlayer>().canNornamAttack--;
        this.GetComponent<StatsPlayer>().skillActive = 0;
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
