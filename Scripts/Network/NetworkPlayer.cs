using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{

    public GameObject camera;
    //public GameObject range;
    private Rigidbody test;
    //public LayerMask layer;

    Vector3 realposition = Vector3.zero;
    Quaternion realrotation = Quaternion.identity;

    public bool walking = false;
  
    void Start()
    {
        if (isLocalPlayer)
        {
          
            gameObject.tag = "OwnerPlayer";
            gameObject.layer = LayerMask.NameToLayer("ownerplayer");
            GameObject objnewObject = (GameObject)Instantiate(camera,
                                                             new Vector3(this.transform.position.x, this.transform.position.y + 5, this.transform.position.z - 10f),
                                                             Quaternion.Euler(this.transform.rotation.x + 26.56505f, this.transform.rotation.y, this.transform.rotation.y));
           // objnewObject.transform.parent = this.transform;
            //CharacterController playercollider = this.GetComponent<CharacterController>();
            /*playercollider.height = 0.0f;
            playercollider.radius = 0.25f;
            playercollider.center = new Vector3( playercollider.center.x, 0.39f, 0.1f);
            */
            objnewObject.GetComponent<CameraController>().player = this.gameObject;

            //GameObject rangeobj = (GameObject)Instantiate(range);
            //rangeobj.GetComponent<rangetrack>().player = this.gameObject;
            GameObject.Find("CHAT").GetComponent<ChatGui>().PLAYER = this.gameObject;
            GameObject.Find("GUIplayer").GetComponent<GUIController>().Setplayer(this.gameObject);
            

        }
        else
        {


        }

    }


}