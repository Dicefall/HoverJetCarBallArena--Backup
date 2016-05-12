using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {
    Camera MainCam;

    float height;

    GameObject[] players;
    GameObject[] pointballs;

    // Use this for initialization
    void Start () {
        MainCam = gameObject.GetComponent<Camera>();
        MainCam.transform.position = new Vector3(0, 80, 0);
        MainCam.transform.rotation = Quaternion.Euler(90, 0, 0);
        //height = 40;

        players = GameObject.FindGameObjectsWithTag("Player");
        pointballs = GameObject.FindGameObjectsWithTag("pointball");
    }

    // Update is called once per frame
    void Update() {

        Vector3 center = new Vector3(0, 0, 0);

        Vector3 toppest = new Vector3(0, 0, 0);
        Vector3 lowest = new Vector3(0, 0, 0);
        Vector3 leftest = new Vector3(0, 0, 0);
        Vector3 rightest = new Vector3(0, 0, 0);


        for (int i = 0;i<players.Length;i++)
        {
            center += players[i].transform.position;

            //Checking toppest position
            if (players[i].transform.position.z > toppest.z)
                toppest = players[i].transform.position;

            //Checking lowest position
            if (players[i].transform.position.z < lowest.z)
                lowest = players[i].transform.position;

            //checking leftest position
            if (players[i].transform.position.x < leftest.x)
                leftest = players[i].transform.position;

            //checking rightest position
            if (players[i].transform.position.x > rightest.x)
                rightest = players[i].transform.position;
        }

        for(int i = 0;i<pointballs.Length;i++)
        {
            center += pointballs[i].transform.position;

            //Checking toppest position
            if (pointballs[i].transform.position.z > toppest.z)
                toppest = pointballs[i].transform.position;

            //checking lowest position
            if (pointballs[i].transform.position.z < lowest.z)
                lowest = pointballs[i].transform.position;

            //Checking leftest position
            if (pointballs[i].transform.position.x < leftest.x)
                leftest = pointballs[i].transform.position;

            //Checking rightest position
            if (pointballs[i].transform.position.x > rightest.x)
                rightest = pointballs[i].transform.position;

        }

        center /= (players.Length + pointballs.Length);
        float maxCloseUp = 30;
        float angle = 180 - (MainCam.GetComponent<Camera>().fieldOfView / 2) - 90;
        height = ((rightest.x - leftest.x) / 2) + ((toppest.z - lowest.z)/2) * Mathf.Tan((angle * (Mathf.PI / 180)));
        if (height < maxCloseUp)
            height = maxCloseUp;

        Vector3 newPOS =
            Vector3.Lerp(MainCam.transform.position,new Vector3(center.x,height,center.z), 0.05f);

        MainCam.transform.position = newPOS;

    }


    static public void SetNewCameraPOS(Vector3 newPOS)
    {
        //Vector3 oldPOS = MainCam.transform.position;
        //camPOS = newPOS;

        

        //camPOS = newPOS;
        
    }
}
