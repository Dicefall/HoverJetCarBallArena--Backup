using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum BallState
{
    FLEE, NONE
}

public class BallPoint : MonoBehaviour {


    public ParticleSystem ballParticles;
    public ColorType BallColor;
    BallState state;
    
    GameObject[] Players = null;

    Vector3 dirVector;

    bool bIncreaseTime = false;
    float fIncreaseTime = 0.0f;

    AudioSource BallAudio;

    public List<AudioClip> BallTouchedSounds = new List<AudioClip>();

    //Lifetime before respawning to another location
    float lifeTime;

	// Use this for initialization
	void Start () {
        BallAudio = GetComponent<AudioSource>();
        ReSpawnBall();
        
        state = BallState.NONE;
    }
	
	// Update is called once per frame
	void Update () {

        CheckPlayers();

        switch (state)
        {
            case BallState.FLEE:
                
                Flee();
                break;
            default:
                break;
        }
        lifeTime -= Time.deltaTime;

        if(lifeTime<=0 || transform.position.y < - 1)
        {
            ReSpawnBall();
        }
	}


    void OnCollisionEnter(Collision collision)
    {
        //Getting the HoverCarBeHaviour of the collided object to do some 'affecting'
        HoverCarBehaviour collidedPlayer = collision.gameObject.GetComponent<HoverCarBehaviour>();
        
        if(collision.gameObject.tag == "Player")
        {
            //PLays a random sound effect that is loaded into the Balls
            BallAudio.PlayOneShot(BallTouchedSounds[Random.Range(0,BallTouchedSounds.Count - 1)]);

            if (BallColor == ColorType.WHITE)
            { 
                    collidedPlayer.giveScore(3);
                    bIncreaseTime = true;
                    fIncreaseTime = 5.0f;
            }

            //If the color of the ball is the same as the ball, then give it an advantage
            else if (collidedPlayer.PlayerColor == BallColor)
            {
                collidedPlayer.Advantage(HoverCarBehaviour.ADVANTAGE.FASTER);
                collidedPlayer.giveScore(1);
            }
            //if not, then give it an impedence
            else
            {
                collidedPlayer.Impedence(HoverCarBehaviour.IMPEDENCE.SLOW);
            }
            ReSpawnBall();
        }
    }

    public void ReSpawnBall()
    {
        //Generating a random position for the ball to spawn at
        GameObject currentlevel = GameObject.FindGameObjectWithTag("Level");
        float sizeX = currentlevel.GetComponentInChildren<BoxCollider>().bounds.size.x;
        float sizeZ = currentlevel.GetComponentInChildren<BoxCollider>().bounds.size.z;
        float posX = currentlevel.transform.position.x;
        float posZ = currentlevel.transform.position.z;
        float height = 2.0f;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        bool respawnOK = false;
        Vector3 RandomPOS = new Vector3(0, 0, 0);

        //Loop until a valid spot is found
        while (!respawnOK)
        {
            RandomPOS = new Vector3(
                Random.Range((posX - sizeX / 2) + 10, (posX + sizeX / 2) - 10),
                height,
                Random.Range((posZ - sizeZ / 2) + 10, (posZ + sizeZ / 2) - 10)
                );

            //Making sure the ball doesnt spawn too close to a player or on top of them
            for (int i = 0; i < players.Length; i++)
            {
                if ((players[i].transform.position - RandomPOS).magnitude < 15)
                {
                    break;
                }
                else
                    respawnOK = true;
            }

        }

        transform.position = RandomPOS;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //Every respawn will give the ball a random new lifetime
        lifeTime = Random.Range(10,18);
    }

    //Checking players position and color to activated certain ball AI when conditions are met
    void CheckPlayers()
    {
        if(Players == null)
            Players = GameObject.FindGameObjectsWithTag("Player");
        else
        { 
            foreach (GameObject player in Players)
            {
                if (player.GetComponent<HoverCarBehaviour>().PlayerColor == BallColor || BallColor == ColorType.WHITE)
                { 
                    if((player.transform.position - transform.position).magnitude < 25)
                    {
                        dirVector = 
                            (
                            new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) -
                            new Vector3(transform.position.x, transform.position.y, transform.position.z) 
                            )
                            .normalized;
                        state = BallState.FLEE;
                    }
                    else
                    {
                        state = BallState.NONE;
                    }
                }
            }
        }
    }

    //The game core will constantly check this method and if it is true.
    //If true the GameCore will increase the time and switch it to false so it doesnt
    //add anymore time to it and the ball doesnt need to ever use this method.
    public float increaseTime()
    {
        if (bIncreaseTime)
        {
            bIncreaseTime = false;
            return fIncreaseTime;
        }
        else
            return 0;
    }

    //Fleeing behaviour for ball. No wall checking.
    void Flee()
    {
        
        float fleeSpeed = 7;

        if(BallColor == ColorType.WHITE)
        {
            fleeSpeed *= 3;
        }

        transform.position = Vector3.Lerp(transform.position, transform.position + -dirVector * fleeSpeed, 0.025f);
        

    }
}
