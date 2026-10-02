using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menuController : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Button_StartGame()
    {
        Debug.Log("Start button pressed");
        SceneManager.LoadScene(1);
    }

     public void Button_ConfirmQuit()
    {
        Debug.Log("Quit button pressed");

        Application.Quit();
    }
}

