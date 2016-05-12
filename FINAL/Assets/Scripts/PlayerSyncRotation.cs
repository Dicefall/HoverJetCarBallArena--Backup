using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour 
{
    [SyncVar]
    private Quaternion syncRotation;
    //[SerializeField] Transform myTransform;
    [SerializeField] float lerpRate = 15;

    Quaternion lastRot;
    float threshold = 0.5f;

    // Use this for initialization
    void Start ()
    {
        
    }
    
    // Update is called once per frame
    void Update ()
    {
        TransmitRotation();
        LerpRotation();
    }
    
    void LerpRotation()
    {
        if (!isLocalPlayer)
        {
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, syncRotation, Time.deltaTime*lerpRate);
        }
    }
    
    //Command functions are called on the client and execute on the server only
    //Function must start with 'Cmd'
    [Command]
    void CmdProvidePositionToServer (Quaternion rot)
    {
        syncRotation = rot;
    }
    
    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            //optimize the network updates to broadcast 
            //only when rotated passed a certain angle threshold
            if(Quaternion.Angle(transform.rotation, lastRot) > threshold)
            {
                CmdProvidePositionToServer(gameObject.transform.rotation);
                lastRot = transform.rotation;

                Debug.Log("Rotation Synced to Server");
            }
        }
    }
}
