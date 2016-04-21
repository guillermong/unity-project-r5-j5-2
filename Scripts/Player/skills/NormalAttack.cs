using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NormalAttack : NetworkBehaviour
{

    public int skillID = 99;
    public float damage = 10.0f;
    public int range = 1;
    public bool autoAttack;
    public LayerMask mask2;
    public NetworkInstanceId playerIdAtacked;
    public Animator animatorPlayer;
    public GameObject rangeNodePrefab;

    private RaycastHit hit; 
    private GameObject playerattacked;
    private Grid grid;

    public GameObject rangePrefab;
    public GameObject displayRange;

    void Start()
    { 
        grid = GameObject.Find("A*").GetComponent<Grid>();
        autoAttack = true;
    }
	
    void Update()
    {
         if (isLocalPlayer)
         {   
                
                if (Input.GetMouseButtonDown(0) && 
                    Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, this.GetComponent<StatsPlayer>().mask) &&
                    Camera.main.GetComponent<CameraController>().mode == 1)
                {
                    if (Camera.main.GetComponent<CameraController>().isSkillActive == 0 && (hit.transform.tag == "Player" || hit.transform.tag == "Monster")) 
                    {                    
                        playerattacked = hit.transform.gameObject;
                        CmdSetPlayerIdAttacked(skillID, playerattacked.GetComponent<NetworkIdentity>().netId);

                        /* if (displayRange == null) { 
                     displayRange = (GameObject)Instantiate(rangePrefab);
                     displayRange.GetComponent<rangetrack>().player = this.gameObject;
                     displayRange.GetComponent<rangetrack>().range = range;
                        }
                         if (GetComponent<StatsPlayer>().displayRangeSkillActive)
                             displayRange.GetComponent<rangetrack>().display();
                         else displayRange.GetComponent<rangetrack>().undisplay();*/
                    }
                    //if (displayRange != null && this.GetComponent<StatsPlayer>().skillActive != skillID) Destroy(displayRange);
                }              
        }
    }

    [Command]
    public void CmdSetPlayerIdAttacked(int skillID, NetworkInstanceId playerID)
    {
        if (playerIdAtacked != playerID || this.GetComponent<StatsPlayer>().skillActive != skillID) {

            if (this.GetComponent<StatsPlayer>().canNornamAttack == 0) { 
                playerIdAtacked = playerID;
                playerattacked = NetworkServer.FindLocalObject(playerIdAtacked);
                this.GetComponent<StatsPlayer>().skillActive = skillID;
                StopCoroutine("attack");
                StartCoroutine("attack");
            }

        }
    }


    [Server]
    public IEnumerator attack()
    {
        //temporalko
        if(this.GetComponent<StatsPlayer>().canWalk == 0)
        {
        this.GetComponent<Unit>().move(grid.NodeFromWorldPoint(playerattacked.transform.position).worldPosition, range);
        this.GetComponent<Unit>().RpcMoveClient(range, grid.NodeFromWorldPoint(playerattacked.transform.position).worldPosition,transform.position);
        }

        while (this.GetComponent<StatsPlayer>().skillActive == skillID) { 

            
            if (!playerattacked.GetComponent<StatsPlayer>().death){

                Node auxTarget = grid.NodeFromWorldPoint(playerattacked.transform.position);
                if (Pathfinding2.GetDistance2(grid.NodeFromWorldPoint(this.transform.position), auxTarget) > range 
                    && !this.GetComponent<NetworkPlayer>().walking
                    && this.GetComponent<StatsPlayer>().canWalk == 0)
                {
                    if (auxTarget.walkable)
                    {
                        this.GetComponent<Unit>().move(auxTarget.worldPosition, range);
                        this.GetComponent<Unit>().RpcMoveClient(range, auxTarget.worldPosition,transform.position);
                    
                    }
                }
                else if (Pathfinding2.GetDistance2(grid.NodeFromWorldPoint(this.transform.position), auxTarget) <= range && !this.GetComponent<NetworkPlayer>().walking)
                {

                    if (!Physics.Linecast(transform.position, playerattacked.transform.position,mask2))
                    {
                        animatorPlayer.Play("atkMode");
                        RpcAnimationatk(playerIdAtacked);

                        yield return new WaitForSeconds( 0.5f / this.GetComponent<StatsPlayer>().speedatk);
                        if (this.GetComponent<StatsPlayer>().skillActive == skillID)
                            playerattacked.GetComponent<StatsPlayer>().TakeDamage(damage, this.GetComponent<NetworkIdentity>().netId);
                        if (!autoAttack)
                            this.GetComponent<StatsPlayer>().skillActive = 0;

                    }
                }



            }
            else 
            { 
                this.GetComponent<StatsPlayer>().skillActive = 0;
            }
            yield return null;
        }

    }

    [ClientRpc]
    public void RpcAnimationatk(NetworkInstanceId playerID)
    {
        GetComponent<Unit>().StopCoroutine("FollowPath");
        this.transform.LookAt(ClientScene.FindLocalObject(playerID).transform.position);
        this.GetComponent<Unit>().rotationPlayer.eulerAngles = this.transform.eulerAngles;
        animatorPlayer.Play("atkMode");
    }

}
