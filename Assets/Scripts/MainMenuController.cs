using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
