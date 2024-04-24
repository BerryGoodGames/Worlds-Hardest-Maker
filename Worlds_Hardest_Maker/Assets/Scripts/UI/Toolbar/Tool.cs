using MyBox;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [DisplayInspector] [InitializationField] [MustBeAssigned] public EditMode ToolEditMode;

    [Separator] [OverrideLabel("Fade Tween")] [SerializeField] private AlphaTween anim;
    [SerializeField] private SelectionSquare selectionSquare;

    [HideInInspector] public bool IsSelected;

    [HideInInspector] public bool InOptionbar;

    public MouseOverUIRect MouseOverUIRect { get; private set; }

    private void Awake() => InOptionbar = transform.parent.CompareTag("OptionContainer");

    private void Start()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();

        OnExitAnchorAttach();

        AnchorAttachManager.OnEnterAttachMode += OnEnterAnchorAttach;
        AnchorAttachManager.OnExitAttachMode += OnExitAnchorAttach;
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
        if (selectionSquare == null) return;

        selectionSquare.SetSelected(selected);

        IsSelected = selected;

        if (!InOptionbar || !IsSelected) return;

        Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
        parentTool.SubSelected(true);
    }

    public void SubSelected(bool subselected) => selectionSquare.SetSubSelected(subselected);

    private void Update() => anim.SetVisible(IsSelected || (MouseOverUIRect.Over && !ReferenceManager.Instance.Menu.activeSelf));

    private void SetVisible(bool visible) => gameObject.SetActive(visible);

    private void OnEnterAnchorAttach() => SetVisible(ToolEditMode.AnchorAvailable);
    private void OnExitAnchorAttach() => SetVisible(ToolEditMode.DefaultAvailable);

    private void OnDestroy()
    {
        AnchorAttachManager.OnEnterAttachMode -= OnEnterAnchorAttach;
        AnchorAttachManager.OnExitAttachMode -= OnExitAnchorAttach;
    }
}