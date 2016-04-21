using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {


    public string playerPrefabName = "Monk";
    public Transform spawnPoint;
    public string roomName = "RoomName";

    private const string VERSION = "v0.0.1";
    private RoomInfo[] roomsList;

	// Use this for initialization
	void Start () {
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings(VERSION);
        print("Connected: " + PhotonNetwork.connectionState); 
	}

    void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom( roomName, roomOptions, TypedLobby.Default);

    }

    void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
        //player.GetComponentInChildren<TextMesh>().text = "aaaaaaaaaaaaaaaaa";
    }

	// Update is called once per frame
	/*void Update () {
	
	}

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }*/
}

