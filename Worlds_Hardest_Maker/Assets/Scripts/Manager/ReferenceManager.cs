using MyBox;
using TMPro;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }
    
    #region Objects
    
    [Foldout("Objects")] public Canvas Canvas;
    
    [Foldout("Objects")] public GameObject Menu;
    [Foldout("Objects")] public AlphaTween MenuTween;
    
    [Foldout("Objects")] public AlphaTween KeybindBlocker;
    [Foldout("Objects")] public TMP_Text KeybindBlockerText;
    
    #endregion
    
    #region Anchor
    
    [Foldout("Anchor")] public RectTransform AnchorBlockChainContainer;
    
    [Foldout("Anchor")] public RectTransform AnchorBlockSourceContainer;
    
    [Foldout("Anchor")] public AnchorBlockQuickMenuController AnchorBlockQuickMenu;
    
    [Foldout("Anchor")] public ChainController MainChainController;
    
    [Foldout("Anchor")] public CustomFitter CustomFitter;
    
    [Foldout("Anchor")] public AnchorBlockConnectorController AnchorBlockConnectorController;
    
    [Foldout("Anchor")] public AnchorBlockPreviewController AnchorBlockPreview;
    
    [Foldout("Anchor")] public AnchorBlockPeriblockerController AnchorBlockPeriblocker;
    
    [Foldout("Anchor")] public AnchorCameraJumping AnchorCameraJumping;
    
    [Foldout("Anchor")] public AlphaTween AnchorNoAnchorSelectedScreen;
    
    [Foldout("Anchor")] public AlphaTween AnchorInPlayModeScreen;
    
    #endregion
    
    #region Materials
    
    [Foldout("Material")] public Material DashedLineMaterial;
    
    #endregion
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}