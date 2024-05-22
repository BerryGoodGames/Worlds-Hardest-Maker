using DG.Tweening;
using MyBox;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    
    [SerializeField] [InitializationField] [Required] private LoadingScreen loadingScreen;
    [Space] [SerializeField] [InitializationField] [Required] private MoveRelativeTween levelhubOpenTween;
    [SerializeField] [InitializationField] [Required] private MoveRelativeTween optionsOpenTween;
    [SerializeField] [InitializationField] [Required] private MoveRelativeTween creditsEnterTween;
    [SerializeField] [InitializationField] [Required] private MoveRelativeTween creditsExitTween;
    [SerializeField] [InitializationField] [Required] private TMP_Text creditsButtonText;
    [SerializeField] [InitializationField] [Required] private MoveRelativeTween levelHubButtonExitTween;
    
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
        
        if (isCreditsOpen)
        {
            levelHubButtonExitTween.Move();
        }
        
        creditsButtonText.text = isCreditsOpen ? "Back" : "Credits";
    }
    
    public void QuitGame() => Application.Quit();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}