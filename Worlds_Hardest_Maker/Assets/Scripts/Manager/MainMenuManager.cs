using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartEditor()
    {
        SceneManager.LoadScene(1);
    }

    public void StartMultiplayer()
    {
        // TODO make it nicer ig
        SceneManager.LoadScene(2);
    }

    public void OpenOptions()
    {
        // TODO idk
        throw new NotImplementedException();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
