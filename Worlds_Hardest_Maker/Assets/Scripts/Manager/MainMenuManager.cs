using MyBox;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] [InitializationField] [MustBeAssigned] private LoadingScreen loadingScreen;
    [Space] [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween levelhubOpenTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween optionsOpenTween;

    public void OpenLevelScene() => loadingScreen.LoadScene(1);

    public void StartMultiplayer() => loadingScreen.LoadScene(2);

    public void OnLevelsClicked() => levelhubOpenTween.Move();

    public void OnOptionsClicked() => optionsOpenTween.Move();

    public void QuitGame() => Application.Quit();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}