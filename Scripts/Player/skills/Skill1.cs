using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class Skill1 : NetworkBehaviour
{

    public int skillID = 1;
    public float cooldown = 3.0f;
    public float damage= 70;
    public int range = 3;
    public KeyCode KeySkill;

    public LayerMask mask;
    public Sprite skillbarSprite;


    [SyncVar(hook = "CoolDownHook")]
    public float cooldown_curring = 0.0f;
    public AudioClip cargar;
    public AudioClip disparar;

    public GameObject laserPrefab;
    public GameObject displayHitprefab;

    private AudioSource managerSound;
    private Ray ray;
    
    
    GameObject playerattacked;
	// Use this for initialization

    
    //private LineRenderer laser;


	void Start () {
        managerSound = GetComponent<AudioSource>();
        KeySkill = KeyCode.Alpha1;
        ///laser = GetComponent<LineRenderer>();
        //laser.enabled = false;

	}

	void LateUpdate () {


            if (isLocalPlayer && Camera.main != null && !GetComponent<StatsPlayer>().ONGUI)
            {

                if (Input.GetKeyDown(KeySkill) && cooldown_curring == 0 && GetComponent<StatsPlayer>().canskill == 0)
                {
                    
                    if (Camera.main.GetComponent<CameraController>().mode == 1 || Camera.main.GetComponent<CameraController>().mode == 2)
                        Camera.main.GetComponent<CameraController>().SetMode3();
                    this.GetComponent<StatsPlayer>().CmdSkillActive(skillID);
                    managerSound.PlayOneShot(cargar, 0.4f);
                    
                }

                else if (Camera.main.GetComponent<CameraController>().mode == 3 && Input.GetMouseButton(0) && this.GetComponent<StatsPlayer>().skillActive == skillID)
                {


                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                    CmdSetatkskill1(skillID, Camera.main.transform.position, Camera.main.transform.forward);
                    managerSound.PlayOneShot(disparar, 0.6f);                    
                    Camera.main.GetComponent<CameraController>().SetMode1();
                }
                else if (Camera.main.GetComponent<CameraController>().mode == 3 && (Input.anyKeyDown || this.GetComponent<StatsPlayer>().canskill != 0))
                {
                    Camera.main.GetComponent<CameraController>().SetMode1();
                    this.GetComponent<StatsPlayer>().CmdSkillActive(0);
                }        
            }
	}

    [Command]
    public void CmdSetatkskill1(int skillID,Vector3 origin, Vector3 direction)
    {
        StartCoroutine(disparo(skillID, origin, direction));    
    }

    [Server]
    IEnumerator disparo(int skillID, Vector3 origin, Vector3 direction)
    {
        yield return new WaitForFixedUpdate();     
        //this.GetComponent<Unit>().stop = true;
        Ray ray2 = new Ray(origin, direction);
        ray = ray2;

        if (this.GetComponent<StatsPlayer>().skillActive == skillID && GetComponent<StatsPlayer>().canskill == 0 && cooldown_curring == 0)
        {


            RaycastHit[] hitinfo = Physics.RaycastAll(ray, 100, mask).OrderBy(h => h.distance).ToArray(); ;
            Vector3 lastHit = Vector3.zero;
            bool hitenemy = false;
            bool hitsomething = false;
            foreach (RaycastHit hit in hitinfo)
            {
                if ((hit.transform.tag == "Player" && !hit.transform.gameObject.GetComponent<StatsPlayer>().death) || hit.transform.tag == "Monster" && hit.transform.gameObject != this.transform.gameObject)
                {

                    playerattacked = hit.transform.gameObject;
                    playerattacked.GetComponent<StatsPlayer>().TakeDamage(damage, this.GetComponent<NetworkIdentity>().netId);
                    hitenemy = true;

                }else if(hit.transform.tag == "Untagged")
                {
                    lastHit = hit.point;
                    hitsomething = true;
                    break;
                }

                
            }

            RpcInitLaserPrefab(origin, direction, lastHit, hitsomething, this.GetComponent<NetworkIdentity>().netId, hitenemy);
            this.GetComponent<StatsPlayer>().skillActive = 0;
            StartCoroutine(CoolDown());
            /*RaycastHit hitinfo;
            
            if (Physics.Raycast(ray, out hitinfo,100, mask))
            {
                if ((hitinfo.transform.tag == "Player" && !hitinfo.transform.gameObject.GetComponent<StatsPlayer>().death) || hitinfo.transform.tag == "Monster")
                {

                    playerattacked = hitinfo.transform.gameObject;
                    print(hitinfo.transform.tag);
                    playerattacked.GetComponent<StatsPlayer>().TakeDamage(damage, this.GetComponent<NetworkIdentity>().netId);
                    managerSound.PlayOneShot(disparar, 0.6f);
                    RpcInitLaserPrefab(origin, direction, hitinfo.point, true, this.GetComponent<NetworkIdentity>().netId, true);
                }
                else
                    RpcInitLaserPrefab(origin, direction, hitinfo.point, true, this.GetComponent<NetworkIdentity>().netId, false);
            }
            else
            {
                RpcInitLaserPrefab(origin, direction, hitinfo.point, false, this.GetComponent<NetworkIdentity>().netId, false);
            }
            this.GetComponent<StatsPlayer>().skillActive = 0;
            StartCoroutine(CoolDown());*/
        }


    }

    [ClientRpc]
    public void RpcInitLaserPrefab(Vector3 origin, Vector3 direction, Vector3 hitpoint, bool hitsusses, NetworkInstanceId id, bool hitenemy)
    {
        this.GetComponent<StatsPlayer>().animatorPlayer.SetInteger("estado", 3);

        GameObject aux = Instantiate(laserPrefab) as GameObject;
        Transform auxtransform = aux.transform;
        aux.transform.SetParent(this.transform);
        aux.transform.localPosition =  laserPrefab.transform.localPosition;

        LineRenderer line = aux.GetComponent<LineRenderer>();
        Ray ray3 = new Ray(origin, direction);

        line.SetPosition(0, ray3.origin);

        if (hitsusses)
            line.SetPosition(1, hitpoint);
        else
            line.SetPosition(1, ray3.GetPoint(100));

        Destroy(aux, 0.3f);


        if (isLocalPlayer && this.GetComponent<NetworkIdentity>().netId == id)
        {
            GameObject displayHit = Instantiate(displayHitprefab) as GameObject;
            displayHit.transform.position = new Vector3(0.75f,0.25f,0);
            GUIText guitext = displayHit.GetComponent<GUIText>();
            if (hitenemy)
            {
                guitext.text = "HIT";
                guitext.color = Color.yellow;

            }else
                guitext.text = "MISS";

            Destroy(displayHit, 1.0f);
            
        }
            

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

