using UnityEngine;

public class Tool : MonoBehaviour
{
    public EditMode ToolEditMode;

    [HideInInspector] public bool IsSelected;

    [HideInInspector] public bool InOptionbar;

    private SelectionSquare selectionSquare;
    private AlphaTween anim;
    private MouseOverUIRect mouseOverUIRect;

    private void Awake()
    {
        InOptionbar = transform.parent.CompareTag("OptionContainer");
    }

    private void Start()
    {
        anim = GetComponent<AlphaTween>();
        mouseOverUIRect = GetComponent<MouseOverUIRect>();
        selectionSquare = transform.GetChild(1).GetComponent<SelectionSquare>();
    }

    public void SwitchGameMode(bool setEditModeVariable)
    {
        ToolbarManager.DeselectAll();
        SetSelected(true);
        if (setEditModeVariable) EditModeManagerOther.Instance.CurrentEditMode = ToolEditMode;
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

    private void Update() => anim.SetVisible(IsSelected || (mouseOverUIRect.Over && !ReferenceManager.Instance.Menu.activeSelf));
}