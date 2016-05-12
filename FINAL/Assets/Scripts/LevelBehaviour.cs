using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelBehaviour : MonoBehaviour {

    public List<GameObject> levelList = new List<GameObject>();
    private GameObject currentLevel;
    private int currentLevelNum;

    GameObject[] players = null;
    GameObject[] balls = null;
    // Use this for initialization
    void Start () {

    }

	// Update is called once per frame
	void Update () {
	    
	}

    //Switching levels
    public void GoToLevel(int levelNum)
    {
        //Destroys the present level
        DestroyImmediate(GameObject.FindGameObjectWithTag("Level"));

        //Loads the new level from the leveList
        currentLevel = levelList[levelNum];
        Instantiate(currentLevel);
        currentLevelNum = levelNum;

        //Grabbing all the players and balls to initiate their respawn when a new level is loaded
        players = GameObject.FindGameObjectsWithTag("Player");
        balls = GameObject.FindGameObjectsWithTag("pointball");

        foreach (GameObject player in players)
            player.GetComponent<HoverCarBehaviour>().RespawnPlayer();

        foreach (GameObject ball in balls)
            ball.GetComponent<BallPoint>().ReSpawnBall();
    }

    public int getLevelCount()
    {
        return levelList.Count;
    }

    public int getCurrentLevel()
    {
        return currentLevelNum;
    }
}
