using System;
using MyBox;
using UnityEngine;

public partial class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }
    
    [ReadOnly] public bool InAttachMode;
    private static readonly int editingString = Animator.StringToHash("Editing");
    
    public static event Action OnEnterAttachMode = () => { };
    public static event Action OnExitAttachMode = () => { };
    
    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.Playing
            || !LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated
            || AnchorManager.Instance.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;
        
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, false, false);
        
        PanelManager.Instance.SetPanelOpen(ReferenceManager.Instance.AnchorPanelController, false, false);
        
        InAttachMode = true;
        
        LevelSessionEditManager.Instance.CurrentEditMode = EditModeManager.Ball;
        
        HighlightAnchor(AnchorManager.Instance.SelectedAnchor);
        
        OnEnterAttachMode.Invoke();
    }
    
    public void ExitAttachMode()
    {
        bool isModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        
        if (LevelSessionEditManager.Instance.Editing)
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, false, false);
        
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        if (!isModeAnchorRelated) PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.LevelSettingsPanelController, false);
        
        InAttachMode = false;
        
        if (AnchorManager.Instance.SelectedAnchor)
        {
            AnchorManager.Instance.SelectedAnchor.GetComponent<Animator>().SetBool(editingString, isModeAnchorRelated);
            AnchorManager.Instance.SelectedAnchor.SetLinesActive(isModeAnchorRelated);
        }
        
        Dehighlight(AnchorManager.Instance.SelectedAnchor);
        
        OnExitAttachMode.Invoke();
    }
    
    public static Transform GetCurrentAnchorContainer() => Instance.InAttachMode ? AnchorManager.Instance.SelectedAnchor.AttachmentContainer : null;
    
    private void Start() =>
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            if (InAttachMode) ExitAttachMode();
        };
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}