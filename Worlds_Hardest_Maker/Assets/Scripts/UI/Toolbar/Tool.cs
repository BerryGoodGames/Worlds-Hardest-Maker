using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// REF
public class Tool : MonoBehaviour
{
    public GameManager.EditMode toolName;
    [HideInInspector] public bool selected;
    [HideInInspector] public bool inOptionbar;

    private MouseOverUI mouseOverUI;
    private SelectionSquare selectionSquare;

    private void Start()
    {
        mouseOverUI = GetComponent<MouseOverUI>();
        selectionSquare = transform.GetChild(1).GetComponent<SelectionSquare>();

        inOptionbar = transform.parent.CompareTag("OptionContainer");

        mouseOverUI.onHovered = () => SetTweenVisibility(!GameManager.Instance.Menu.activeSelf);
        mouseOverUI.onUnhovered = () => SetTweenVisibility(false);
    }

    public void SwitchGameMode(bool setEditModeVariable)
    {
        ToolbarManager.DeselectAll();
        Selected(true);
        if(setEditModeVariable) GameManager.Instance.CurrentEditMode = toolName;
    }

    public void SwitchGameMode()
    {
        SwitchGameMode(true);
    }

    public void Selected(bool selected)
    {
        selectionSquare.Selected(selected);

        this.selected = selected;

        if (inOptionbar && selected)
        {
            Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
            parentTool.SubSelected(true);
        }

        SetTweenVisibility(selected);
    }
    public void SubSelected(bool subselected)
    {
        selectionSquare.SubSelected(subselected);
    }

    public void SetTweenVisibility(bool visible)
    {
        AlphaTweenUI tween = GetComponent<AlphaTweenUI>();
        tween.SetVisibility(visible);

        print($"{visible} {transform.name}");
    }
}
