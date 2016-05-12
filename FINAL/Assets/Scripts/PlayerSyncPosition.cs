using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSyncPosition : NetworkBehaviour 
{
    [SyncVar]
    private Vector3 syncPos;

    //[SerializeField] Transform myTransform;
    [SerializeField] float lerpRate = 15;

    Vector3 lastPos;
    float threshold = 0.5f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        TransmitPosition();
        LerpPosition();
	}

    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            gameObject.transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime*lerpRate);
        }
    }

    void SyncCallBack(Vector3 pos)
    {
      //  Debug.Log("callback!!");
    }

    //Command functions are called on the client and execute on the server only
    //Function must start with 'Cmd'
    [Command]
    void CmdProvidePositionToServer (Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            //optimize the network updates to broadcast only when a certain distance threshold is passed
            if(Vector3.Distance(transform.position, lastPos) > threshold)
            {
                CmdProvidePositionToServer(gameObject.transform.position);
                lastPos = transform.position;

                Debug.Log("Position Synced to Server");
            }
        }
    }
}
