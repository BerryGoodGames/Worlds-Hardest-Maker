using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

public class Tool : MonoBehaviour
{
    [DisplayInspector] [InitializationField] [Required] public EditMode ToolEditMode;
    
    [Separator] [OverrideLabel("Fade Tween")] [SerializeField] private AlphaTween anim;
    [FormerlySerializedAs("selectionSquare")] [SerializeField] private ToolSelectionSquare toolSelectionSquare;
    
    [HideInInspector] public bool IsSelected;
    
    [HideInInspector] public bool InOptionbar;
    
    public MouseOverUIRect MouseOverUIRect { get; private set; }
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    private void Awake() => InOptionbar = transform.parent.CompareTag("OptionContainer");
    
    private void Start()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();
        
        OnExitAnchorAttach(new ExitAnchorAttachEvent());
        
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
    }
    
    public void SwitchGameMode(bool setEditModeVariable)
    {
        ToolbarManager.DeselectAll();
        SetSelected(true);
        if (setEditModeVariable) LevelSessionEditManager.Instance.CurrentEditMode = ToolEditMode;
    }
    
    public void SwitchGameMode() => SwitchGameMode(true);
    
    public void SetSelected(bool selected)
    {
        if (toolSelectionSquare == null) return;
        
        toolSelectionSquare.SetSelected(selected);
        
        IsSelected = selected;
        
        if (!InOptionbar || !IsSelected) return;
        
        Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
        parentTool.SubSelected(true);
    }
    
    public void SubSelected(bool subselected) => toolSelectionSquare.SetSubSelected(subselected);
    
    private void Update() => anim.SetVisible(IsSelected || (MouseOverUIRect.Over && !ReferenceManager.Instance.Menu.activeSelf));
    
    private void SetVisible(bool visible) => gameObject.SetActive(visible);
    
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => SetVisible(ToolEditMode.AnchorAvailable);
    private void OnExitAnchorAttach(ExitAnchorAttachEvent evt) => SetVisible(ToolEditMode.DefaultAvailable);
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
    }
}