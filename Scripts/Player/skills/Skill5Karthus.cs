using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Skill5Karthus : NetworkBehaviour
{

    public NetworkInstanceId playerOwner;
    public float damage = 10.0f;
    public LayerMask mask;
    public float radius = 1;

	// Use this for initialization
	void Start () {
        if(isServer)
            StartCoroutine("pegar");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    [Server]
    IEnumerator pegar()
    {

        yield return new WaitForSeconds(0.3f);
        Collider[] hitplayers = Physics.OverlapSphere(this.transform.position, (0.5f * radius) / 2, mask);

        foreach (Collider players in hitplayers)
        {
            if ((players.gameObject.tag == "Player" || players.gameObject.tag == "OwnerPlayer") && players != this.GetComponent<CharacterController>())
            {
                players.transform.gameObject.GetComponent<StatsPlayer>().TakeDamage(damage, playerOwner);

            }
        }
    }
}
