using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [MustBeAssigned] private LoadingScreen loadingScreen;
    [Space] [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween levelhubOpenTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween optionsOpenTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween creditsEnterTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween creditsExitTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private TMP_Text creditsButtonText;
    [SerializeField] [InitializationField] [MustBeAssigned] private MoveRelativeTween levelHubButtonExitTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private Button levelHubButton;
    [SerializeField] [InitializationField] [MustBeAssigned] private Button optionsButton;
    
    private bool isCreditsOpen;
    private Tween creditsTween;
    
    public void OpenLevelScene() => loadingScreen.LoadScene(1);
    
    public void StartMultiplayer() => loadingScreen.LoadScene(2);
    
    public void OnLevelsClicked() => levelhubOpenTween.Move();
    
    public void OnOptionsClicked() => optionsOpenTween.Move();
    
    public void OnCreditsClicked()
    {
        if (creditsTween != null && creditsTween.IsActive() && creditsTween.IsPlaying()) return;
        
        isCreditsOpen = !isCreditsOpen;
        
        creditsTween = (isCreditsOpen ? creditsEnterTween : creditsExitTween).MoveAndReturn();
        
        if (isCreditsOpen) levelHubButtonExitTween.Move();
        
        creditsButtonText.text = isCreditsOpen ? "Back" : "Credits";
        
        levelHubButton.interactable = !isCreditsOpen;
        optionsButton.interactable = !isCreditsOpen;
    }
    
    public void QuitGame() => Application.Quit();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}