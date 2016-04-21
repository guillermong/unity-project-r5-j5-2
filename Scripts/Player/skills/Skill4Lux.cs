using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill4Lux : NetworkBehaviour
{

    public float moveSpeed = 1.5f;
    public float Distance;
    public float timeStuck = 2.0f;
    public Vector3 velocity;
    public NetworkInstanceId playerOwner;
    public float damage = 10.0f;

	// Use this for initialization
	void Start () {
        Distance = 0;
	}

    private void FixedUpdate()
    {
        // we want the bullet to be updated only on the server
        if (!base.isServer)
            return;

        // transform bullet on the server
        
        transform.position += velocity * Time.deltaTime * moveSpeed;
        Distance += transform.position.x;
    }

	// Update is called once per frame
	void Update () {
	



	}

    void OnCollisionEnter(Collision col)
    {
        if (isServer)
        {
            if (col.gameObject.tag == "Player" && col.gameObject.GetComponent<NetworkIdentity>().netId != playerOwner)
            {
                col.gameObject.GetComponent<BuffsDebuffsPlayer>().StartCoroutine("SetStuck",timeStuck);
                col.gameObject.GetComponent<StatsPlayer>().TakeDamage(damage, playerOwner);
                
            }
        }
            

    }
}
