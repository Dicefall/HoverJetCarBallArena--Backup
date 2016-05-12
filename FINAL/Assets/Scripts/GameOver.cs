using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour {

    public Text GameOverText;
    
	// Use this for initialization
	void Start () {
        GameOverText.text = "Final Score: " + HoverCarBehaviour.singlePlayerScore.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
