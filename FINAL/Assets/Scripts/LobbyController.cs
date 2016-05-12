using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyController : NetworkBehaviour {

    NetworkLobbyManager lobbyManager;
    GameObject ThisHoverCar;
    // Use this for initialization
    void Start ()
    {
        GameObject netManager = GameObject.Find("NetworkObject");
        NetworkLobbyManager lobbyManager = netManager.GetComponent<NetworkLobbyManager>();
        lobbyManager.gamePlayerPrefab = lobbyManager.spawnPrefabs[Random.Range(0, 5)];

        if (isLocalPlayer)
        {

        }
        else
        {

        }
        // lobbyManager = GameObject.FindGameObjectWithTag("NetworkObject").GetComponent<NetworkLobbyManager>();
        //Debug.Log(lobbyManager);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
