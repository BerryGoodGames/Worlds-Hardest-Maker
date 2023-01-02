using System;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [HideInInspector] public EditMode toolName;
    [SerializeField] private string toolType;
    [HideInInspector] public bool selected;
    [HideInInspector] public bool inOptionbar;
    private SelectionSquare selectionSquare;
    private AlphaUITween anim;
    private MouseOverUI mouseOverUI;

    private void Awake()
    {
        if (!Enum.TryParse(toolType, out toolName))
            Debug.LogError($"{toolType} was not a valid type");

        inOptionbar = transform.parent.CompareTag("OptionContainer");
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
        Selected(true);
        if (setEditModeVariable) EditModeManager.Instance.CurrentEditMode = toolName;
    }

    public void SwitchGameMode()
    {
        SwitchGameMode(true);
    }

    public void Selected(bool selected)
    {
        if (selectionSquare == null) return;

        selectionSquare.Selected(selected);

        this.selected = selected;

        if (!inOptionbar || !selected) return;

        Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
        parentTool.SubSelected(true);
    }

    public void SubSelected(bool subselected)
    {
        selectionSquare.SubSelected(subselected);
    }

    private void Update()
    {
        anim.SetVisible(selected || (mouseOverUI.over && !ReferenceManager.Instance.menu.activeSelf));
    }
}