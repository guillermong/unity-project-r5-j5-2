using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Move2 : NetworkBehaviour
{

    public int skillID = 0;
    public LayerMask mask;
    public Animator spritePlayer;

    private Grid grid;
    private Vector3 move; 
    private RaycastHit hit;

    private Vector3 nextpoint;

    public KeyCode KeyMove1;
    public KeyCode KeyMove2;
    public KeyCode KeyMove3;
    public KeyCode KeyMove4;

	void Start () {

        grid = GameObject.Find("A*").GetComponent<Grid>();
        nextpoint = Vector3.zero;
        StartCoroutine(move2());

        KeyMove1 = KeyCode.A;
        KeyMove2 = KeyCode.S;
        KeyMove3 = KeyCode.D;
        KeyMove4 = KeyCode.W;
	}
	

    IEnumerator move2( ){
        if (isLocalPlayer )
        {

            while (true) {
                if ((Input.GetKey(KeyMove1) || Input.GetKey(KeyMove2) || Input.GetKey(KeyMove3) || Input.GetKey(KeyMove4)) && !GetComponent<StatsPlayer>().ONGUI)
                {

                    Vector2 positionNode = new Vector2(0, 0);

                    if (Input.GetKey(KeyMove4))
                        positionNode += new Vector2(Mathf.RoundToInt(Camera.main.transform.forward.x), Mathf.RoundToInt(Camera.main.transform.forward.z));

                    else if (Input.GetKey(KeyMove2))
                        positionNode += new Vector2(Mathf.RoundToInt(Camera.main.transform.forward.x * -1), Mathf.RoundToInt(Camera.main.transform.forward.z * -1));

                    if (Input.GetKey(KeyMove3))
                        positionNode += new Vector2(Mathf.RoundToInt(Camera.main.transform.right.x), Mathf.RoundToInt(Camera.main.transform.right.z));

                    else if (Input.GetKey(KeyMove1))
                        positionNode += new Vector2(Mathf.RoundToInt(Camera.main.transform.right.x * -1), Mathf.RoundToInt(Camera.main.transform.right.z * -1));

                    Node nodeStart = grid.NodeFromWorldPoint(this.transform.position);
                    int checkX = nodeStart.gridX + (int)positionNode.x;
                    int checkY = nodeStart.gridY + (int)positionNode.y;

                    Node nodeEnd = grid.grid[checkX, checkY];

                    

                    Vector3 nextPosition = new Vector3(nodeEnd.worldPosition.x, -1.0f, nodeEnd.worldPosition.z);

                    if (nodeEnd.walkable && !Physics.Raycast(nextPosition, Vector3.up, out hit, Mathf.Infinity, mask) /*&& nextpoint != nodeEnd.worldPosition*/)
                    {
                        //if (this.GetComponent<StatsPlayer>().canWalk == 0) { 
                            this.GetComponent<Unit>().CmdMoveServer(0, nodeEnd.worldPosition);
                        //}
                        
                    }else
                        spritePlayer.SetInteger("estado", 1);
                }
                yield return new WaitForSeconds(0.1f);
                //yield return null;  
            }


	    }
	}
}
