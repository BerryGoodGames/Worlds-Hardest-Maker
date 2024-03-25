using System;
using System.Windows.Forms;
using MyBox;
using UnityEngine;

public class AnchorAttachManager : MonoBehaviour
{
    public static AnchorAttachManager Instance { get; private set; }

    [ReadOnly] public bool InAttachMode;
    
    public void EnterAttachMode()
    {
        if (LevelSessionEditManager.Instance.Playing
            || !LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated
            || AnchorManager.Instance.SelectedAnchor == null
            || AnchorPositionInputEditManager.Instance.IsEditing) return;

        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, true);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, false, false);

        InAttachMode = true;
    }

    public void ExitAttachMode()
    {
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachButtonController, false, false);
        PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        
        InAttachMode = false;
    }

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            if (!InAttachMode) return;
            
            PanelManager.Instance.SetPanelHidden(ReferenceManager.Instance.AnchorAttachExitButtonController, true);
        
            InAttachMode = false;
        };
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
