using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;


public class Unit : NetworkBehaviour
{
    [SyncVar]
    public bool stop;
    public int rango = 0;
    public int PreRango = 0;
    public int targetIndex;

    public Animator spritePlayer;

    NetworkPlayer walking;
    Vector3[] path;

    public Action SkillAction;
    private StatsPlayer unitparent;
    public Quaternion rotationPlayer;
    

    void Start()
    {
        stop = false;
        walking = GetComponent<NetworkPlayer>();
        unitparent = GetComponentInParent<StatsPlayer>();
        targetIndex = 0;

    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationPlayer, 10f);
    }

    //SKILL CANIMAR (MOUSE O ASDW)
    [Command]
    public void CmdMoveServer(int _rango,Vector3 targetPosition)
    {
        if (!this.GetComponent<StatsPlayer>().death /*&& this.GetComponent<StatsPlayer>().canWalk == 0*/) { 
            this.GetComponent<StatsPlayer>().skillActive = 0;
            move(targetPosition,_rango);
            RpcMoveClient( _rango,targetPosition,this.transform.position);
        }
    }

    [ClientRpc]
    public void RpcMoveClient(int _rango, Vector3 targetPosition,Vector3 realPosition)
    {
        this.transform.position = realPosition;
        move(targetPosition, _rango);
    }

    public void move( Vector3 target, int _rango) {
        PreRango = _rango;
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);       
    }

    [Server]
    public void move2(Vector3 target, int _rango, Action _skillAction)
    {
        PreRango = _rango;
        SkillAction = _skillAction;
        PathRequestManager.RequestPath(transform.position, target, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            targetIndex = 0;
            rango = PreRango;
            path = newPath;
            StopCoroutine("FollowPath");

            if (path.Length <= rango)
            {
                walking.walking = false;
                if (!this.GetComponent<InBattleController>().inbattle)
                    spritePlayer.SetInteger("estado", 1);
                else
                    spritePlayer.SetInteger("estado", 3);
                if (SkillAction != null)
                {
                    SkillAction();
                    SkillAction = null;
                }
                if (path.Length > 0)
                {
                    this.transform.LookAt(path[path.Length - 1]);
                    rotationPlayer.eulerAngles = this.transform.eulerAngles;
                }
                
            }
            else 
            { 
                StartCoroutine("FollowPath");
            }
        }
    }

    IEnumerator FollowPath()
    {
        if (path.Length != 0) {
            
            Vector3 currentWaypoint = path[0];
            while (true)
            {

                if (transform.position == currentWaypoint)
                {
                    targetIndex++;

                    if (targetIndex + rango >= path.Length)
                    {
                        if (!(Input.GetKey(GetComponent<Move2>().KeyMove1) || Input.GetKey(GetComponent<Move2>().KeyMove2) || Input.GetKey(GetComponent<Move2>().KeyMove3) || Input.GetKey(GetComponent<Move2>().KeyMove4)))
                        {
                            if (!this.GetComponent<InBattleController>().inbattle)
                                spritePlayer.SetInteger("estado", 1);
                            else
                                spritePlayer.SetInteger("estado", 3);
                            walking.walking = false;
                        }

                        if (SkillAction != null)
                        {
                            SkillAction();
                            SkillAction = null;
                        }


                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                if (stop == true)
                {
                    stop = false;
                    walking.walking = false;
                    if (!this.GetComponent<StatsPlayer>().death) {
                        if (!this.GetComponent<InBattleController>().inbattle)
                            spritePlayer.SetInteger("estado", 1);
                        else
                            spritePlayer.SetInteger("estado", 3);
                    }
                    yield break;
                }

                if (GetComponent<StatsPlayer>().canWalk == 0) 
                { 
                    rotationPlayer = Quaternion.LookRotation(currentWaypoint - transform.position);
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, GetComponent<StatsPlayer>().speed * Time.deltaTime);
                    walking.walking = true;
                    spritePlayer.SetInteger("estado", 2);
                }
                else
                {
                    walking.walking = false;
                }
                    
                yield return null;         
            }
        }
        yield return null;
    }

    
    public void OnDrawGizmos()
    {
        if (path != null)
        {

            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(1,1,1));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}






/*walking.walking = false;
if(!this.GetComponent<StatsPlayer>().inbattle)
    spritePlayer.SetInteger("estado", 1);
else
    spritePlayer.SetInteger("estado", 3);*/
/*public IEnumerator rotatePlayer() {

    Quaternion rotation = Quaternion.LookRotation(currentWaypoint - transform.position);
    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.1f);
}*/
//transform.position = Vector3.Lerp(transform.position, currentWaypoint, 6f);
//transform.rotation = Quaternion.Lerp(transform.rotation, rotationPlayer, 0.1f);
//transform.rotation = Quaternion.Lerp(transform.rotation , Quaternion.FromToRotation(transform.forward,currentWaypoint -transform.position),0.1f);
//transform.LookAt(currentWaypoint);
//transform.rotation = Quaternion.Lerp(transform.rotation , Quaternion.FromToRotation(transform.forward,currentWaypoint -transform.position),0.1f);
