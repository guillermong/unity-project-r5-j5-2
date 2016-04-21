using UnityEngine;
using System.Collections;

public class FacePlayerHead : MonoBehaviour {

    private Transform cameraPlayer;
    private Animator animator;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        //ESTO SIRVE PARA QUE EL SPRITE ESTE SIEMPRE DE FRENTE A LA CAMARA PERO LA CAMARA NO DEBE SER HIJO
        if (cameraPlayer != null)
        {
            //this.transform.rotation = cameraPlayer.rotation;


            print(this.transform.localEulerAngles);

            Vector3 test3 = this.transform.localEulerAngles;


            if (test3.y > 157.5f && test3.y <= 202.5f)
            {
                animator.Play("headup");
                sprite.flipX = false;
            }
            else if (test3.y > 112.5f && test3.y <= 157.5f)
            {
                animator.Play("head3.4");
                sprite.flipX = false;
            }
            else if (test3.y > 67.5f && test3.y <= 112.5f)
            {
                animator.Play("headleft");
                sprite.flipX = false;
            }
            else if (test3.y > 22.5f && test3.y <= 67.5f)
            {
                animator.Play("Monk3.4back");
                sprite.flipX = false;
            }
            else if (test3.y > 337.5f || test3.y <= 22.5f)
            {
                animator.Play("headback");
                sprite.flipX = false;
            }
            else if (test3.y > 292.5f && test3.y <= 337.5f)
            {
                animator.Play("head3.4back");
                sprite.flipX = true;
            }
            else if (test3.y > 247.5f && test3.y <= 292.5f)
            {
                animator.Play("headleft");
                sprite.flipX = true;
            }
            else if (test3.y > 202.5f && test3.y < 247.5f)
            {
                animator.Play("head3.4");
                sprite.flipX = true;
            }




        }
        else
        {
            cameraPlayer = Camera.main.transform;
        }
    }
}
