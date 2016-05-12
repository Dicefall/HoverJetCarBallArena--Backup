using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public enum GameModeType
{
    SINGLEPLAYER,MULTIPLAYER
}
//There are 5 colors with the exception of white, just being a ball class, that each ball and player corresponds with.
public enum ColorType
{
    RED,GREEN,BLUE,ORANGE,PURPLE,WHITE
}

public class GameCore : NetworkBehaviour {

   
    
    public List<GameObject> Players = new List<GameObject>();
    public List<GameObject> PointBalls = new List<GameObject>();

    //playerBehaviour and ball point is grabbed after the player and ball has spawned.
    //Ease of life to access their own screen instead of GetComponent on each use.
    List<HoverCarBehaviour> playerBehaviour = new List<HoverCarBehaviour>();
    List<BallPoint> ballPoint = new List<BallPoint>();

    public Camera GameCam;
 

    public Text PlayerColorScore;
    public Text TimeText;
    public Text FinalCount;

    bool bShowScore = false;

    //When to change level
    int levelChangeCheck = 3;

    //Increment the levelChangeCheck
    int levelChangeCheckIncrement = 3;
    int totalScore = 0;


    public GameObject levels;
    LevelBehaviour levelBehaviour;



    float timeLimit = 6000.0f;

    // Use this for initialization
    void Start () {
        
        
        //Getting Players and Spawning
        //for (int i = 0;i<Players.Count;i++)
        //{ 
        //    if (Players[i].GetComponent<HoverCarBehaviour>().PlayerColor == GameMenu.PlayerSingle)
        //    { 
        //        Instantiate(Players[i]);
        //        break;
        //    }
        //}
        GameObject[] playersBehaviour = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0;i<playersBehaviour.Length;i++)
            playerBehaviour.Add(playersBehaviour[i].GetComponent<HoverCarBehaviour>());

        //Getting balls and spawning
        for (int i = 0; i < PointBalls.Count; i++)
            Instantiate(PointBalls[i]);

        GameObject[] balls = GameObject.FindGameObjectsWithTag("pointball");

        for (int i = 0; i < balls.Length; i++)
            ballPoint.Add(balls[i].GetComponent<BallPoint>());


        Instantiate(GameCam);

        //Starting level
        levelBehaviour = levels.GetComponent<LevelBehaviour>();
        levelBehaviour.GoToLevel(0);
        
    }

    // Update is called once per frame
    void Update()
    {

        timeLimit -= Time.deltaTime;
        Addtime();

        //DEBUGGING
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (HoverCarBehaviour player in playerBehaviour)
               { 
                    player.RespawnPlayer();
                    break;
                }
        }

        if (Input.GetKey(KeyCode.C))
        {
            foreach (BallPoint ball in ballPoint)
                ball.ReSpawnBall();
        }

        if (Input.GetKey(KeyCode.V))
        {
            ChangeLevel();

        }

        if(Input.GetKey(KeyCode.Tab))
        {
            bShowScore = true;
            
        }

        if(Input.GetKeyUp(KeyCode.Tab))
        {
            bShowScore = false;
        }

        DisplayScore();
        DisplayTexts();


        //totalScore is set to zero after every call for checking if all the points the players
        //acquired is passed the levelChangeCheck
        foreach (HoverCarBehaviour player in playerBehaviour)
        {
            totalScore += player.getScore();
        }

        //levelChangeCheckIncrement increases everytime the level has changed
        if (totalScore >= levelChangeCheck)
        {
            levelChangeCheck += levelChangeCheckIncrement;
            //ChangeLevel();

        }
        totalScore = 0;

        //Goto gameover scene
        if (timeLimit <= 0)
        {
            Application.LoadLevel(5);
        }
    }

    void ChangeLevel()
    {

            bool diffLevel = false;
            int newLevel = 0;
            
            //Checks to make sure the same level doesnt load
            while (!diffLevel)
            {
                newLevel = Random.Range(0, levelBehaviour.getLevelCount());
                if (newLevel != levelBehaviour.getCurrentLevel())
                    diffLevel = true;
            }
            levelBehaviour.GoToLevel(newLevel);

    }

    void Addtime()
    {
        //For single player
        foreach (BallPoint ball in ballPoint)
            timeLimit += ball.increaseTime();
    }

    void DisplayTexts()
    {

        if (timeLimit <= 5.0f)
        {
            FinalCount.text = ((int)timeLimit + 1).ToString();
            TimeText.text = " ";
        }
        else
        {
            TimeText.text = timeLimit.ToString();
            FinalCount.text = " ";
        }



    }
    void DisplayScore()
    {
        if(bShowScore)
        { 
            if (GameMenu.GameMode == GameModeType.MULTIPLAYER)
            {
                foreach (HoverCarBehaviour player in playerBehaviour)
                {
                    switch (player.PlayerColor)
                    {
                        case ColorType.BLUE:
                            PlayerColorScore.text = "Player Blue: " + player.getScore().ToString();
                            break;
                        case ColorType.GREEN:
                            PlayerColorScore.text = "Player Green: " + player.getScore().ToString();
                            break;
                        case ColorType.ORANGE:
                            PlayerColorScore.text = "Player Orange: " + player.getScore().ToString();
                            break;
                        case ColorType.PURPLE:
                            PlayerColorScore.text = "Player Purple: " + player.getScore().ToString();
                            break;
                        case ColorType.RED:
                            PlayerColorScore.text = "Player Red: " + player.getScore().ToString();

                            break;
                        default:
                            break;

                    }

                }

            }

        }
        else
        {
            PlayerColorScore.text = " ";
        }

        //In Single player the score is always shown.
        if (GameMenu.GameMode == GameModeType.SINGLEPLAYER)
        {
            PlayerColorScore.text = "Player Score: " + HoverCarBehaviour.singlePlayerScore.ToString();
        }
    }
}
