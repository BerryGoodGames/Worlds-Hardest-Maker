using System;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private LoadingScreen loadingScreen;

    public void StartEditor()
    {
        loadingScreen.LoadScene(1);
    }

    public void StartMultiplayer()
    {
        loadingScreen.LoadScene(2);
    }

    public void OpenOptions()
    {
        // TODO add game options
        throw new NotImplementedException();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}