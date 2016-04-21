using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class FacePlayerSystem : MonoBehaviour {

    public GameObject head1;

    private Transform cameraPlayer;
    private Animator animator;
    private SpriteRenderer sprite;

    private Animator animatorHead;
    private SpriteRenderer spriteHead;
    private Transform transHead;
    private StatsPlayer unitparent;

    private Dictionary<int, string> head;
    private Dictionary<int, string> normal;
    private Dictionary<int, string> walk;
    private Dictionary<int, string> atk;
    private Dictionary<int, string> battle;

    Dictionary<int, Dictionary<int, string>> AnimationList;

	// Use this for initialization
	void Start () {


        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        animatorHead = head1.GetComponent<Animator>();
        spriteHead = head1.GetComponent<SpriteRenderer>();
        transHead = head1.GetComponent<Transform>();

        unitparent = GetComponentInParent<StatsPlayer>();

        head= new Dictionary<int, string>();
        normal = new Dictionary<int, string>();
        walk = new Dictionary<int, string>();
        atk = new Dictionary<int, string>();
        battle = new Dictionary<int, string>();

        head[1] = "headup";
        head[2] = "head3.4";
        head[3] = "headleft";
        head[4] = "head3.4back";
        head[5] = "headback";
        head[6] = "head3.4back";
        head[7] = "headleft";
        head[8] = "head3.4";

        normal[1] = "Monkup";
        normal[2] = "Monk3.4";
        normal[3] = "Monkleft";
        normal[4] = "Monk3.4up";
        normal[5] = "Monkback";
        normal[6] = "Monk3.4up";
        normal[7] = "Monkleft";
        normal[8] = "Monk3.4";

        walk[1] = "Monkwalkup";
        walk[2] = "Monkwalk3.4";
        walk[3] = "Monkwalkleft";
        walk[4] = "Monkwalk3.4up";
        walk[5] = "Monkwalkback";
        walk[6] = "Monkwalk3.4up";
        walk[7] = "Monkwalkleft";
        walk[8] = "Monkwalk3.4";

        atk[1] = "monkatkup";
        atk[2] = "monkatkup";
        atk[3] = "monkatkup";
        atk[4] = "monkatkback";
        atk[5] = "monkatkback";
        atk[6] = "monkatkback";
        atk[7] = "monkatkback";
        atk[8] = "monkatkup";

        battle[1] = "monkbattleup";
        battle[2] = "monkbattleup";
        battle[3] = "monkbattleup";
        battle[4] = "monkbattleback";
        battle[5] = "monkbattleback";
        battle[6] = "monkbattleback";
        battle[7] = "monkbattleback";
        battle[8] = "monkbattleup";


        
        //DICCIONARIO DE DICCIONARIO

        AnimationList = new Dictionary<int, Dictionary<int, string>> { 
            {1, normal},
            {2, walk},
            {3, atk},
            {4, battle}      
        };	
	}
	
	// Update is called once per frame
    //void LateUpdate () {
	void Update () {
        this.transform.rotation = Camera.main.transform.rotation;
        int rotatinoPlayer = getRotationPlayer();

        if (animator.GetInteger("estado") <= 2)
        {
            if(rotatinoPlayer <= 5 )
                sprite.flipX = false;
            else
                sprite.flipX = true;
        }
        else
        {
            if (rotatinoPlayer >= 5)
                sprite.flipX = false;
            else
                sprite.flipX = true;

        }

        if (rotatinoPlayer <= 5)
            spriteHead.flipX = false;
        else
            spriteHead.flipX = true;

        //animator.SetInteger("estado", unitparent.estadoAnimacion);
        animator.SetInteger("rotation", rotatinoPlayer);
        animatorHead.SetInteger("rotation", rotatinoPlayer);
        setRotationHead();
        transHead.localPosition = new Vector3(0.036f, 0.403f, transHead.localPosition.z);
	}


    int getRotationPlayer()
    {
        if (Camera.main != null)
        {
            Vector3 playerRotation = this.transform.localEulerAngles;

            if (playerRotation.y > 157.5f && playerRotation.y <= 202.5f) 
                return 1;
            else if (playerRotation.y > 112.5f && playerRotation.y <= 157.5f)
                return 2;
            else if (playerRotation.y > 67.5f && playerRotation.y <= 112.5f)
                return 3;
            else if (playerRotation.y > 22.5f && playerRotation.y <= 67.5f)
                return 4;
            else if (playerRotation.y > 337.5f || playerRotation.y <= 22.5f)
                return 5;
            else if (playerRotation.y > 292.5f && playerRotation.y <= 337.5f)
                return 6;
            else if (playerRotation.y > 247.5f && playerRotation.y <= 292.5f)
                return 7;
            else if (playerRotation.y > 202.5f && playerRotation.y < 247.5f)
                return 8;
            
        }
        return 0;
	}

    void setRotationHead()
    {
        if (Camera.main != null)
        {
            Vector3 playerRotation = this.transform.localEulerAngles;

            if (playerRotation.y > 157.5f && playerRotation.y <= 202.5f)
                transHead.localPosition = new Vector3(0.036f, 0.403f, transHead.localPosition.z);
            else if (playerRotation.y > 112.5f && playerRotation.y <= 157.5f)
                transHead.localPosition = new Vector3(0.014f, 0.38f, transHead.localPosition.z);
            else if (playerRotation.y > 67.5f && playerRotation.y <= 112.5f)
                transHead.localPosition = new Vector3(0.0f, 0.318f, transHead.localPosition.z);
            else if (playerRotation.y > 22.5f && playerRotation.y <= 67.5f)
                transHead.localPosition = new Vector3(-0.011f, 0.334f, transHead.localPosition.z);
            else if (playerRotation.y > 337.5f || playerRotation.y <= 22.5f)
                transHead.localPosition = new Vector3(0.016f, 0.347f, transHead.localPosition.z);
            else if (playerRotation.y > 292.5f && playerRotation.y <= 337.5f)
                transHead.localPosition = new Vector3(-0.011f, 0.334f, transHead.localPosition.z);
            else if (playerRotation.y > 247.5f && playerRotation.y <= 292.5f)
                transHead.localPosition = new Vector3(0.0f, 0.318f, transHead.localPosition.z);
            else if (playerRotation.y > 202.5f && playerRotation.y < 247.5f)
                transHead.localPosition = new Vector3(-0.012f, 0.38f, transHead.localPosition.z);

        }
    }
}
