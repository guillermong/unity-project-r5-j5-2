using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager2 : MonoBehaviour {

    public Transform player;
    string registeredName = "somekindofuniquename";
    float refreshRequestLength = 3.0f;
    //HostData[] hostData;
    public string chosenGameName = "PathoftheMonk";
    public NetworkPlayer myPlayer;

    private void StartServer()
    {
        //Network.InitializeServer(16, Random.Range(2000, 2500), !Network.HavePublicAddress());
        //MasterServer.RegisterHost(registeredName, chosenGameName);
    }

    void OnServerInitialized()
    {
        //if (Network.isServer)
        //{
            //myPlayer = Network.player;
            //makePlayer(myPlayer);
        //}
    }

    void OnConnectedToServer()
    {
       // myPlayer = Network.player;
       // networkView.RPC("makePlayer", RPCMode.Server, myPlayer);
    }

 
}
