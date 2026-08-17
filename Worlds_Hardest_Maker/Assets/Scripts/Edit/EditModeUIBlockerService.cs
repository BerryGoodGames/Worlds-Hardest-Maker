using MyBox;
using UnityEngine;

public class EditModeUIBlockerService : MonoBehaviour, IEditModeUIBlockerService
{
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween toolbarTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween infobarEditTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween playButtonTween;
    
    public void BlockAndDisable()
    {
        // block menu from opening
        MenuManager.Instance.BlockMenu = true;
        
        // disable panels
        toolbarTween.SetPlay(true);
        infobarEditTween.SetPlay(true);
        playButtonTween.TweenToY(-125, false);
    }

    public void ReleaseAndShow()
    {
        // release menu
        MenuManager.Instance.BlockMenu = false;
        
        // show panels
        toolbarTween.SetPlay(LevelSessionEditManager.Instance.IsPlaying);
        infobarEditTween.SetPlay(LevelSessionEditManager.Instance.IsPlaying);
        playButtonTween.SetPlay(LevelSessionEditManager.Instance.IsPlaying);
    }
}