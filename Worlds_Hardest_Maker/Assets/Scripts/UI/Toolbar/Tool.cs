using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Tool : MonoBehaviour
{
    [FormerlySerializedAs("toolName")] [HideInInspector]
    public EditMode ToolName;

    [SerializeField] private string toolType;

    [FormerlySerializedAs("selected")] [HideInInspector]
    public bool Selected;

    [FormerlySerializedAs("inOptionbar")] [HideInInspector]
    public bool InOptionbar;

    private SelectionSquare selectionSquare;
    private AlphaUITween anim;
    private MouseOverUI mouseOverUI;

    private void Awake()
    {
        if (!Enum.TryParse(toolType, out ToolName))
            Debug.LogError($"{toolType} was not a valid type");

        InOptionbar = transform.parent.CompareTag("OptionContainer");
    }

    private void Start()
    {
        anim = GetComponent<AlphaUITween>();
        mouseOverUI = GetComponent<MouseOverUI>();
        selectionSquare = transform.GetChild(1).GetComponent<SelectionSquare>();
    }

    public void SwitchGameMode(bool setEditModeVariable)
    {
        ToolbarManager.DeselectAll();
        IsSelected(true);
        if (setEditModeVariable) EditModeManager.Instance.CurrentEditMode = ToolName;
    }

    public void SwitchGameMode()
    {
        SwitchGameMode(true);
    }

    public void IsSelected(bool selected)
    {
        if (selectionSquare == null) return;

        selectionSquare.Selected(selected);

        Selected = selected;

        if (!InOptionbar || !selected) return;

        Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
        parentTool.SubSelected(true);
    }

    public void SubSelected(bool subselected)
    {
        selectionSquare.SubSelected(subselected);
    }

    private void Update()
    {
        anim.SetVisible(Selected || (mouseOverUI.Over && !ReferenceManager.Instance.Menu.activeSelf));
    }
}