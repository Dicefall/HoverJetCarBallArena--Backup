using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiplayerMenu : NetworkBehaviour {
    public static string CurrentScene;

    string RootPath = "Asset/";

   // public NetworkLobbyManager LM;

    // Use this for initialization
    void Start() {
       // LM.gamePlayerPrefab = LM.spawnPrefabs[0];
	}

	
	// Update is called once per frame
	void Update () {
    }

    void StartServer()
    {

    }


}
