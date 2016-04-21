using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FacePlayerCamera : MonoBehaviour {

    //spublic string[] AnimatorStateNames;

    public GameObject head;

    private Transform cameraPlayer;
    private Animator animator;
    private SpriteRenderer sprite;

    private Animator animatorHead;
    private SpriteRenderer spriteHead;
    private Transform transHead;
    private NetworkPlayer unitparent;

    public int estadoAnimacion = 0;

    
	// Use this for initialization
	void Start () {


        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        animatorHead = head.GetComponent<Animator>();
        spriteHead = head.GetComponent<SpriteRenderer>();
        transHead = head.GetComponent<Transform>();

        unitparent = GetComponentInParent<NetworkPlayer>();


        /*RuntimeAnimatorController test = animator.runtimeAnimatorController;
        AnimatorStateInfo test2 = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo test3 = animator.GetNextAnimatorStateInfo(0);
        Animation test123;
        AnimationClip a5a;
        this.GetComponent<AnimationState>().speed = 6;*/

        //test.animationClips
        
	}

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

    private IEnumerator WaitForAnimation2(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }
	
	// Update is called once per frame
	void Update () {
        //print(animator.GetCurrentAnimatorStateInfo(0));
        if (Camera.main != null)
        {
            this.transform.rotation = Camera.main.transform.rotation;

            Vector3 test3 = this.transform.localEulerAngles;
            bool waling = unitparent.walking;

            if (test3.y > 157.5f && test3.y <= 202.5f) 
            {

                if (waling  )
                {
                    
                    //animator.Play("Monkwalkup");
                    animator.SetTrigger("Monkwalkup");
                }
                else if (Input.GetKey(KeyCode.S) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.SetTrigger("Monkwalkup");
                else
                    animator.Play("Monkup");
                sprite.flipX = false;    
            

                animatorHead.Play("headup");
                spriteHead.flipX = false;
                transHead.localPosition = new Vector3(0.036f, 0.403f, transHead.localPosition.z);

            }
            else if (test3.y > 112.5f && test3.y <= 157.5f)
            {
                if (waling)
                    animator.Play("Monkwalk3.4");
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalk3.4");
                else
                    animator.Play("Monk3.4");

                sprite.flipX = false;
                animatorHead.Play("head3.4");
                spriteHead.flipX = false;
                transHead.localPosition = new Vector3(0.014f, 0.38f, transHead.localPosition.z);
            }
            else if (test3.y > 67.5f && test3.y <= 112.5f)
            {
                if (waling)
                    animator.Play("Monkwalkleft");
                else if (Input.GetKey(KeyCode.A) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalkleft");
                else
                animator.Play("Monkleft");
                sprite.flipX = false;

                animatorHead.Play("headleft");
                spriteHead.flipX = false;
                transHead.localPosition = new Vector3(0.0f, 0.318f, transHead.localPosition.z);
            }
            else if (test3.y > 22.5f && test3.y <= 67.5f)
            {
                if (waling)
                    animator.Play("Monkwalk3.4up");
                else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalk3.4up");
                else
                animator.Play("Monk3.4up");
                sprite.flipX = false;

                animatorHead.Play("head3.4back");
                spriteHead.flipX = false;
                transHead.localPosition = new Vector3(-0.011f, 0.334f, transHead.localPosition.z);
            }
            else if (test3.y > 337.5f || test3.y <= 22.5f)
            {
                if (waling)
                    animator.Play("Monkwalkback");
                else if (Input.GetKey(KeyCode.W) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalkback");
                else
                animator.Play("Monkback");
                sprite.flipX = false;

                animatorHead.Play("headback");
                spriteHead.flipX = false;
                transHead.localPosition = new Vector3(0.016f, 0.347f, transHead.localPosition.z);
            }
            else if (test3.y > 292.5f && test3.y <= 337.5f)
            {
                if (waling)
                    animator.Play("Monkwalk3.4up");
                else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalk3.4up");
                else
                animator.Play("Monk3.4up");
                sprite.flipX = true;

                animatorHead.Play("head3.4back");
                spriteHead.flipX = true;
                transHead.localPosition = new Vector3(-0.011f, 0.334f, transHead.localPosition.z);
            }
            else if (test3.y > 247.5f && test3.y <= 292.5f)
            {
                if (waling)
                    animator.Play("Monkwalkleft");
                else if (Input.GetKey(KeyCode.D) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalkleft");
                else
                animator.Play("Monkleft");
                sprite.flipX = true;

                animatorHead.Play("headleft");
                spriteHead.flipX = true;
                transHead.localPosition = new Vector3(0.0f, 0.318f, transHead.localPosition.z);
            }
            else if (test3.y > 202.5f && test3.y < 247.5f)
            {
                if (waling)
                    animator.Play("Monkwalk3.4");
                else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S) && GetComponentInParent<NetworkIdentity>().isLocalPlayer)
                    animator.Play("Monkwalk3.4");
                else
                animator.Play("Monk3.4");
                sprite.flipX = true;

                animatorHead.Play("head3.4");
                spriteHead.flipX = true;
                transHead.localPosition = new Vector3(-0.012f, 0.38f, transHead.localPosition.z);
            }
            



        }    
	}
}
