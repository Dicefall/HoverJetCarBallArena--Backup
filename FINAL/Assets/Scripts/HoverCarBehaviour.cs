using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



public class HoverCarBehaviour : NetworkBehaviour {

    //For game over scene in singleplayer
    static public int singlePlayerScore;

    public enum IMPEDENCE
    {
        SLOW, NONE
    }

    public enum ADVANTAGE
    {
        FASTER, NONE
    }

    public AudioClip[] audioClips = new AudioClip[5];
    public ParticleSystem ThrusterparticleSys;

    public ParticleSystem FrontLeftThrusterParticle;
    public ParticleSystem FrontRightThrusterParticle;

    public Light FrontLeftThrusterLight;
    public Light FrontRightThrusterLight;

    public Light engineLight;
    public ColorType PlayerColor;

    AudioSource[] hoverCarAudios;

    float hoverHeight = 3.0f;
    float hoverForce = 40.0f;

    Rigidbody rb;

    float hoverCarThrustReg = 120;
    float forwardThrust;

    bool bForward;
    bool bTurnRight;
    bool bTurnLeft;

    bool startMoving;
    bool moving;
    bool startBraking;
    bool braking;


    float minRotation;
    float maxRotation;
    float currentRotation;
    float rotationSpeed;
    

    float currentTurn;
    float turnSpeed;

    int score;

    //Stats for the advantages and impedences
    bool bImpedence;
    bool bAdvantage;
    float impedenceTimer = 2;
    float advantageTimer = 2;
    float impStatusTime = 0;
    float advStatusTime = 0;

    IMPEDENCE eImp;
    ADVANTAGE eAdv;

	// Use this for initialization
	void Start () {

        if (!isLocalPlayer)
            this.enabled = false;

        //PlayerColor = GameMenu.PlayerSingle;
        //PlayerColor = (ColorType)Random.Range(0, 6);

        rb = gameObject.GetComponent<Rigidbody>();
        bForward = false;
        startMoving = false;
        moving = false;
        startBraking = false;
        braking = false;

        //rotation refers to the banking of the player
        rotationSpeed = 14.0f;
        currentRotation = 0.0f;
        minRotation = -90;
        maxRotation = 90;

        //How the actual turn is calculated and made
        currentTurn = 0;
        turnSpeed = rotationSpeed/2;

        forwardThrust = hoverCarThrustReg;

        //0 = idle, 1 = StartMoving, 2 = Moving,3 = BrakeStart, 4 = Braking
        hoverCarAudios = gameObject.GetComponents<AudioSource>();

        for (int i = 0; i < hoverCarAudios.Length;i++)
        {
            hoverCarAudios[i].clip = audioClips[i];
            hoverCarAudios[i].loop = false;
        }

        

        eImp = IMPEDENCE.NONE;
        eAdv = ADVANTAGE.NONE;

        bImpedence = false;
        bAdvantage = false;

        score = 0;

        RespawnPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        CheckInput();
        SoundCheck();
    }

    //Checking and implementing how sound is played
    void SoundCheck()
    {
        if(!moving)
        {
            if (hoverCarAudios[2].isPlaying)
                hoverCarAudios[2].Stop();

            if (hoverCarAudios[1].isPlaying)
                hoverCarAudios[1].Stop();
        }

        if(!braking)
        { 
            if (hoverCarAudios[4].isPlaying)
                hoverCarAudios[4].Stop();


            if (hoverCarAudios[3].isPlaying)
                hoverCarAudios[3].Stop();
        }

        if(!moving && !braking)
        { 
            if (!hoverCarAudios[0].isPlaying)
                hoverCarAudios[0].Play();
        }


        if (startMoving)
        {
            if (hoverCarAudios[0].isPlaying)
                hoverCarAudios[0].Stop();
            if (hoverCarAudios[2].isPlaying)
                hoverCarAudios[2].Stop();

            hoverCarAudios[1].Play();
            startMoving = false;
            moving = true;

        }
        else if (moving)
        {
            if (!hoverCarAudios[1].isPlaying)
            {
                if(!hoverCarAudios[2].isPlaying)
                { 
                    hoverCarAudios[2].loop = true;
                    hoverCarAudios[2].Play();
                }
            }

        }

        if (startBraking)
        {
            if (hoverCarAudios[0].isPlaying)
                hoverCarAudios[0].Stop();
            if (hoverCarAudios[4].isPlaying)
                hoverCarAudios[4].Stop();

            hoverCarAudios[3].Play();

            startBraking = false;
            braking = true;

        }
        else if (braking)
        {
            if (!hoverCarAudios[3].isPlaying)
            {
                if (!hoverCarAudios[4].isPlaying)
                {
                    hoverCarAudios[4].loop = true;
                    hoverCarAudios[4].Play();
                }
            }
        }

    }


    
    void FixedUpdate()
    {
        //Creating the hover effect of the car
        HoverRayCastCheck();


        //When an advantage or impedence has happened
        if (bImpedence)
            Impedence(eImp);
        if (bAdvantage)
            Advantage(eAdv);

        if (bForward)
        {
            rb.AddForce(transform.forward * forwardThrust);
        }
        MoveingEffect();

        if(braking)
        {
            Braking();
        }
        BrakingEffect();

        //Turning the hover car and also banking the vehicle
        if (bTurnRight)
        {
            currentRotation -= rotationSpeed;
            currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

            currentTurn -= turnSpeed;
        }

        if(bTurnLeft)
        {
            currentRotation += rotationSpeed;
            currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

            currentTurn += turnSpeed;
        }

        //If no turning is initiated the car will automatically rotate back to neutral
        if(!bTurnLeft && !bTurnRight)
        {
            currentRotation = Mathf.LerpUnclamped(currentRotation, 0, 0.25f);
        }
        
        //Doing the action rotation onto the car
        transform.rotation = Quaternion.Euler((Vector3.forward * currentRotation) + (Vector3.up * -currentTurn));
        
        //If it ever falls off the level
        if (transform.position.y < -5)
            RespawnPlayer();

        //So it doesnt fly off the level
        if(transform.position.y>4)
        {
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }



    }

    //Doing the impedence
    public void Impedence(IMPEDENCE effect)
    {
        if (bAdvantage)
        { 
            bAdvantage = false;
            advStatusTime = 0;
        }

        if (!bImpedence)
        {
            eImp = effect;
            bImpedence = true;
        }

        if (effect == IMPEDENCE.SLOW)
        {
            forwardThrust = hoverCarThrustReg / 4;
            impStatusTime += Time.deltaTime;
            if(impStatusTime>=impedenceTimer)
            {
                impStatusTime = 0;
                eImp = IMPEDENCE.NONE;
                forwardThrust = hoverCarThrustReg;
            }
        }

        if(effect == IMPEDENCE.NONE)
        {
            bImpedence = false;
        }

        
    }

    //Doing the advantage
    public void Advantage(ADVANTAGE effect)
    {
        if (bImpedence)
        { 
            bImpedence = false;
            impStatusTime = 0;
        }

        if (!bAdvantage)
        {
            eAdv = effect;
            bAdvantage = true;
        }

        if (effect == ADVANTAGE.FASTER)
        {
            forwardThrust = hoverCarThrustReg * 2;
            advStatusTime += Time.deltaTime;
            if (advStatusTime >= advantageTimer)
            {
                advStatusTime = 0;
                eAdv = ADVANTAGE.NONE;
                forwardThrust = hoverCarThrustReg;
            }
        }

        if (effect == ADVANTAGE.NONE)
        {
            bAdvantage = false;
        }
    }

    //***Makes the car hover
    void HoverRayCastCheck()
    {
        Ray frontRay = new Ray(transform.position, -Vector3.up);
        RaycastHit hoverHit;


        if (Physics.Raycast(frontRay, out hoverHit, hoverHeight))
        {
            if(hoverHit.distance<hoverHeight)
            {
                float porportionalHeight = (hoverHeight - hoverHit.distance) / hoverHeight;
                Vector3 hForce = (Vector3.up * porportionalHeight * hoverForce);
                rb.AddForce(hForce,ForceMode.Acceleration);
                
            }
        }
        
    }

    //Returns score of this particular player
    public int getScore()
    {
        if (GameMenu.GameMode == GameModeType.SINGLEPLAYER)
            return singlePlayerScore;
        else if (GameMenu.GameMode == GameModeType.MULTIPLAYER)
            return score;
        else
            return 0;
    }

    //Increases score of player. The BallPoint class calls this, not the player.
    public void giveScore(int points)
    {
        if(GameMenu.GameMode == GameModeType.SINGLEPLAYER)
        {
            singlePlayerScore += points;
        }
        else
        { 
            score += points;
        }
    }

    //Moving particle effects of the player
    void MoveingEffect()
    {
        if(bForward)
        { 
            float thrustStartSpeed = 150;
            float thrustStartSize = 15.0f;
            float engineLightRange = 50;

            if (bAdvantage)
            {
                if (eAdv == ADVANTAGE.FASTER)
                {
                    ThrusterparticleSys.startSpeed = thrustStartSpeed * 2;
                    ThrusterparticleSys.startSize = thrustStartSize * 2;
                    engineLight.range = engineLightRange * 2;
                }

            }
            else if (bImpedence)
            {
                if (eImp == IMPEDENCE.SLOW)
                {
                    ThrusterparticleSys.startSpeed = thrustStartSpeed / 4;
                    ThrusterparticleSys.startSize = thrustStartSize / 4;
                    engineLight.range = engineLightRange / 4;
                }
            }
            else
            {
                ThrusterparticleSys.startSpeed = thrustStartSpeed;
                ThrusterparticleSys.startSize = thrustStartSize;
                engineLight.range = engineLightRange;
            }
        }
        else
        {
            ThrusterparticleSys.startSpeed = 0;
            ThrusterparticleSys.startSize = 5.0f;
            engineLight.range = 0;
        }
    }


    //cheking keyboard inputs for the turning and acceleration
    void CheckInput()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            //For audio
            startMoving = true;

        }

        if (Input.GetKey(KeyCode.W))
        {
            bForward = true;
            
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            bForward = false;
            moving = false;
        }
    

        if(Input.GetKey(KeyCode.A))
        {
            bTurnLeft = true;
        }

        if(Input.GetKeyUp(KeyCode.A))
        {
            bTurnLeft = false;

        }

        if(Input.GetKey(KeyCode.D))
        {
            bTurnRight = true;
        }

        if(Input.GetKeyUp(KeyCode.D))
        {
            bTurnRight = false;

        }

        if(Input.GetKeyDown(KeyCode.Space))
        {

            startBraking = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            braking = false;

        }

    }
    public void RespawnPlayer()
    {
        //if(isLocalPlayer)
        { 
            GameObject currentlevel = GameObject.FindGameObjectWithTag("Level");
            float sizeX = currentlevel.GetComponentInChildren<BoxCollider>().bounds.size.x;
            float sizeZ = currentlevel.GetComponentInChildren<BoxCollider>().bounds.size.z;
            float posX = currentlevel.transform.position.x;
            float posZ = currentlevel.transform.position.z;
            float height = 1.0f;

            Vector3 RandomPOS = new Vector3(
                Random.Range((posX - sizeX / 2) + 10, (posX + sizeX / 2)) - 10,
                height,
                Random.Range((posZ - sizeZ / 2) + 10, (posZ + sizeZ / 2)) - 10
                );
            transform.position = RandomPOS;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    void Braking()
    {
        rb.velocity = Vector3.LerpUnclamped(rb.velocity, new Vector3(0,0,0), 0.05f);
        if(rb.velocity.magnitude < 3)
        {
            braking = false;
        }
    }

    void BrakingEffect()
    {

        if (braking == false)
        {
            FrontLeftThrusterParticle.enableEmission = false;
            FrontRightThrusterParticle.enableEmission = false;

            FrontLeftThrusterLight.enabled = false;
            FrontRightThrusterLight.enabled = false;
        }
        else
        {
            FrontLeftThrusterParticle.enableEmission = true;
            FrontRightThrusterParticle.enableEmission = true;

            FrontLeftThrusterLight.enabled = true;
            FrontRightThrusterLight.enabled = true;
        }
    }
}
