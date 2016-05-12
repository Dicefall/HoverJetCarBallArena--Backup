using UnityEngine;
using System.Collections;


public class GameMenu : MonoBehaviour {


    //All the menu stuff is done in one class with the exception of the gameover screen.
    //Simplicity sake.

    
    public static ColorType PlayerSingle;
    public static GameModeType GameMode;
    float rotation = 0;

    //Main menus before Player selection scene
    public void GoToMultiplayer()
    {
        Application.LoadLevel(3);
    }

    public void GotoPlayerSelection()
    {
        Application.LoadLevel(2);
    }

    public void GotoChooseGameMode()
    {
        Application.LoadLevel(1);
    }

    //Player Selection Scene
    public void ChooseRed()
    {
        PlayerSingle = ColorType.RED;
        Application.LoadLevel(4);
    }

    public void ChooseBlue()
    {
        PlayerSingle = ColorType.BLUE;
        Application.LoadLevel(4);
    }

    public void ChooseGreen()
    {
        PlayerSingle = ColorType.GREEN;
        Application.LoadLevel(4);
    }

    public void ChoosePurple()
    {
        PlayerSingle = ColorType.PURPLE;
        Application.LoadLevel(4);
    }

    public void ChooseOrange()
    {
        PlayerSingle = ColorType.ORANGE;
        Application.LoadLevel(4);
    }
    public void MultiplayerScreenGoBack()
    {
        Application.LoadLevel(1);
    }


    //GameOver scene buttons
    public void Replay()
    {
        Application.LoadLevel(2);
    }
    public void QuitGame()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    //Rotate the camera for effects
    void Update()
    {
        rotation += 0.15f;
        GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
    }
    
    public void GotoLobby()
    {
        Application.LoadLevel(6);
    }
}
