using System;
using MyBox;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private LoadingScreen loadingScreen;

    public void StartEditor() => loadingScreen.LoadScene(1);

    public void StartMultiplayer() => loadingScreen.LoadScene(2);

    public void OpenOptions() => throw
        // TODO add game options
        new NotImplementedException();

    public void QuitGame() => Application.Quit();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}