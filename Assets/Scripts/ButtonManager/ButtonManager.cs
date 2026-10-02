using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    
    public void PlayGameBtn()
    {
        SceneManager.LoadScene(1); // When the player hits the Play Button, it will go to the level scene.
    }

    public void OptionsBtn()
    {
        SceneManager.LoadScene(2); // Redirects the player to the options screen.
    }

    public void QuitGameBtn()
    {
        Application.Quit(); // Exits the game. You can't hit the quit button while in Unity Engine. 
    }

}
