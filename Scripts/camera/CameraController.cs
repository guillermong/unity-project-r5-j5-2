using UnityEngine;
using System.Collections;
using Pathfinding.RVO;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public Camera camerahp;
    public int skillID = 0;
    public Texture2D cursorNormal;
    public Texture2D cursorAttack;
    public Texture2D cursorTarget;
    public Texture2D cursorTargetpoint;
    public Texture2D cursorTargetpoint2;

    public LayerMask mask;
    public LayerMask mask2;
    public LayerMask mask3TargetGround;

    public float catchTime = 0.25f;
    public float speedMouse = 5.0f;

    private Transform targetMouse;
    private Transform targetMouse2;
    private float lastClickTime;
    private Vector3 inicialposition;
    private Quaternion inicialrotation;
    private Camera cameraPlayer;

    public GameObject player;
    public GameObject spritePlayer;
    public GameObject AreaSkill;
    private Vector3 offset;
    private Vector3 inicialpositioncamera;

    private Grid grid;
    public Camera cameraGUI;
    public int isSkillActive = 0;

    public int mode;
    GameObject hitpoint;

    private Color material;
    private float zoom;
    private bool click = false;
    
    

	void Start () {

        Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);
        cameraPlayer = GetComponent<Camera>();

        inicialrotation = this.transform.rotation;
        inicialpositioncamera = transform.position;

        offset = transform.position - player.transform.position;
        inicialposition = offset;

        targetMouse2 = GameObject.Find("Target2").transform;

        hitpoint = GameObject.Find("HitPoint");
        hitpoint.SetActive(false);
        grid = GameObject.Find("A*").GetComponent<Grid>();
        mode = 1;
        material = targetMouse2.GetComponent<MeshRenderer>().material.color;
        spritePlayer = player.transform.FindChild("MonkSprite").gameObject;
        AreaSkill = GameObject.Find("AreaSkill");

        zoom = camerahp.orthographicSize;
        AreaSkill.GetComponent<MeshRenderer>().enabled = false;   
	}
	
	// Update is called once per frame
	void Update () {
        
        

        if (mode == 1) { 
            rotatecameraX();
            rotatecameraY();
            zoomcamera();
            inicialcamera();
            UpdateTargetPosition();
        }
        else if (mode == 2) {

            UpdateTargetPosition();
            rotatecameraXY();
            zoomcamera();
        }
        else if (mode == 3) {

            rotatecameraXY();
     
        }       
       
	}

    void LateUpdate()
    {
        if (player == null)
        {
            hitpoint.SetActive(true);
            Object.Destroy(this.gameObject);
        }
        else
        {
            if (mode == 1)
            {

                transform.position = player.transform.position + offset;
                this.transform.LookAt(player.transform.position);
            }
            else if (mode == 2 || mode == 3)
            {
                transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
            }
        }
    }

    public void SetMode1() {        
        
        cameraPlayer.orthographic = true;
        cameraGUI.orthographic = true;
        hitpoint.SetActive(false);
        Cursor.visible = true;
        spritePlayer.SetActive(true);
        speedMouse = 5.0f;
        if (mode == 3)
            camerahp.orthographicSize = zoom;
        mode = 1;
    }
    public void SetMode2() {
        
        cameraPlayer.orthographic= false;
        cameraGUI.orthographic = false;
        //cameraGUI.nearClipPlane = 1;
        cameraPlayer.fieldOfView = 60;
        cameraGUI.fieldOfView = 60;
        hitpoint.SetActive(false);
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
        this.transform.rotation = player.transform.rotation;
        Cursor.visible = true;
        spritePlayer.SetActive(false);
        speedMouse = 5.0f;
        if (mode == 3)
            camerahp.orthographicSize = zoom;

        mode = 2;
    }

    public void SetMode3()
    {
        mode = 3; 
        cameraPlayer.orthographic = false;
        cameraGUI.orthographic = false;
        //cameraGUI.nearClipPlane = 0.5f;
        hitpoint.SetActive(true);
        cameraPlayer.fieldOfView = 30;
        cameraGUI.fieldOfView = 30;
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z);
        this.transform.rotation = player.transform.rotation;
        Cursor.visible = false;
        spritePlayer.SetActive(false);
        speedMouse = 2.0f;

        zoom = camerahp.orthographicSize;
        camerahp.orthographicSize = 2;

    }

    public void UpdateTargetPosition()
    {
       
        RaycastHit hit;

        if (isSkillActive == 1)
        {
            Physics.Raycast(cameraPlayer.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask3TargetGround);
            targetMouse2.GetComponent<MeshRenderer>().material.color = Color.blue;

            Cursor.SetCursor(cursorTarget, Vector2.zero, CursorMode.Auto);

            Node n = grid.NodeFromWorldPoint(hit.point);
            Vector3 positionactual = new Vector3(n.worldPosition.x, 0.01f, n.worldPosition.z);
            if (n.walkable)
                targetMouse2.position = positionactual;

            positionactual.y = -1.0f;

            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.LeftShift)) isSkillActive = 0;

        }
        else if (isSkillActive == 2)
        {
            Physics.Raycast(cameraPlayer.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask2);
            if (hit.transform.tag == "Player" || hit.transform.tag == "Monster")
            {
                targetMouse2.GetComponent<MeshRenderer>().material.color = Color.red;

                Cursor.SetCursor(cursorTargetpoint, Vector2.zero, CursorMode.Auto);

                Node n = grid.NodeFromWorldPoint(hit.point);
                Vector3 positionactual = new Vector3(n.worldPosition.x, 0.01f, n.worldPosition.z);
                if (n.walkable)
                    targetMouse2.position = positionactual;

                positionactual.y = -1.0f;



            }
            else { 
                targetMouse2.GetComponent<MeshRenderer>().material.color = Color.blue;

                Cursor.SetCursor(cursorTargetpoint2, Vector2.zero, CursorMode.Auto);

                Node n = grid.NodeFromWorldPoint(hit.point);
                Vector3 positionactual = new Vector3(n.worldPosition.x, 0.01f, n.worldPosition.z);
                if (n.walkable)
                    targetMouse2.position = positionactual;

                positionactual.y = -1.0f;

                
            }
            if (Input.GetMouseButton(1)) isSkillActive = 0;
        }else 
        {
            if (Physics.Raycast(cameraPlayer.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask))
            {
           
                targetMouse2.GetComponent<MeshRenderer>().material.color = material;
                if (hit.transform.tag == "Player" || hit.transform.tag == "Monster")
                {
                    Cursor.SetCursor(cursorAttack, Vector2.zero, CursorMode.Auto);

                }
                else
                {
                    Cursor.SetCursor(cursorNormal, Vector2.zero, CursorMode.Auto);

                    Node n = grid.NodeFromWorldPoint(hit.point);
                    Vector3 positionactual = new Vector3(n.worldPosition.x, 0.01f, n.worldPosition.z);
                    if (n.walkable)
                        targetMouse2.position = positionactual;

                    positionactual.y = -1.0f;

                    if (Input.GetMouseButtonDown(0) && !Physics.Raycast(positionactual, Vector3.up, out hit, Mathf.Infinity, mask) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        click = true;
                    }

                    if (click && player.GetComponent<StatsPlayer>().canWalk == 0)
                    {
                        player.GetComponent<Unit>().CmdMoveServer(0, targetMouse2.position);
                        
                        click = false;

                    }
                }            
            }            
         }
        //if (Input.GetMouseButton(2)) player.GetComponent<StatsPlayer>().isCasting = false;

    }

    void rotatecameraX()
    {
        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftControl) && mode == 1)
        {
            Quaternion aux = player.transform.rotation;

            this.transform.RotateAround(player.transform.position, Vector3.up, Input.GetAxis("Mouse X") * speedMouse);
            this.transform.LookAt(player.transform.position);
            offset = transform.position - player.transform.position;

            player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, this.transform.eulerAngles.y, player.transform.eulerAngles.z);
            player.GetComponent<Unit>().rotationPlayer.eulerAngles = new Vector3(player.transform.eulerAngles.x, this.transform.eulerAngles.y, player.transform.eulerAngles.z);
            //MALO LA COMPARACION,  SE COMPARAN LAS MISMAS COSAS
            if (aux != player.transform.rotation)
                player.GetComponent<StatsPlayer>().CmdSyncRotation(player.transform.rotation);
        }
    }

    void rotatecameraY()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1))
        {
            this.transform.RotateAround(player.transform.position, Vector3.left, Input.GetAxis("Mouse Y") * speedMouse);
            this.transform.LookAt(player.transform.position);

            if (this.transform.position.y > 1 && this.transform.position.y < 10.1)
            {              
                offset = transform.position - player.transform.position;
            }

        }
    }

    public void rotatecameraXY() {
        Quaternion aux = player.transform.rotation;

        float x = Input.GetAxis("Mouse X") * speedMouse;
        float y = Input.GetAxis("Mouse Y") * speedMouse;
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x-y,this.transform.eulerAngles.y+x, 0);

        player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, this.transform.eulerAngles.y, player.transform.eulerAngles.z);
        player.GetComponent<Unit>().rotationPlayer.eulerAngles = new Vector3(player.transform.eulerAngles.x, this.transform.eulerAngles.y, player.transform.eulerAngles.z);
        //MALO LA COMPARACION,  SE COMPARAN LAS MISMAS COSAS
        if(aux != player.transform.rotation)
            player.GetComponent<StatsPlayer>().CmdSyncRotation(player.transform.rotation);
    
    }

    void zoomcamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            if (mode == 2) SetMode1();
            else if (cameraPlayer.orthographicSize >= 2 && cameraPlayer.orthographicSize < 8) 
            { 
                cameraPlayer.orthographicSize++;
                cameraGUI.orthographicSize++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            if (cameraPlayer.orthographicSize == 2) SetMode2(); 
            else if (cameraPlayer.orthographicSize > 2 && cameraPlayer.orthographicSize <= 8)
            {
                cameraPlayer.orthographicSize--;
                cameraGUI.orthographicSize--;
            }
            
        }
    }

    void inicialcamera()
    {

        if(Input.GetButtonDown("Fire2"))
        {
            if (Time.time - lastClickTime < catchTime)
            {
                offset = inicialposition;
                this.transform.position = inicialpositioncamera;
                this.transform.rotation = inicialrotation;
                cameraPlayer.orthographicSize= 3;
                cameraGUI.orthographicSize = 3;
            }
        lastClickTime = Time.time;
        }
    }

    public void SetShowArea(bool active, float Area)
    {
        if (active)
        {
            isSkillActive = 1;
            targetMouse2.gameObject.SetActive(false);
            AreaSkill.transform.localScale = new Vector3(0.5f * Area, 0.0001f, 0.5f * Area);
            AreaSkill.GetComponent<MeshRenderer>().enabled = true;           
        }
        else
        {
            //isSkillActive = 0;
            targetMouse2.gameObject.SetActive(true);
            AreaSkill.GetComponent<MeshRenderer>().enabled = false;   
        }
    }
}
