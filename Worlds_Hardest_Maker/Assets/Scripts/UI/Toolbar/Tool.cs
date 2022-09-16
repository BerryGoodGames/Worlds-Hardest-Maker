using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tool : MonoBehaviour
{
    public GameManager.EditMode toolName;
    [HideInInspector] public bool selected;
    [HideInInspector] public bool inOptionbar;
    private void Awake()
    {
        inOptionbar = transform.parent.CompareTag("OptionContainer");
    }

    public void SwitchGameMode(bool setEditModeVariable)
    {
        ToolbarManager.DeselectAll();
        Selected(true);
        if (setEditModeVariable) GameManager.Instance.CurrentEditMode = toolName;
    }
    public void SwitchGameMode()
    {
        SwitchGameMode(true);
    }
    public void Selected(bool selected)
    {
        transform.GetChild(1).GetComponent<SelectionSquare>().Selected(selected);

        this.selected = selected;

        if (inOptionbar && selected)
        {
            Tool parentTool = transform.parent.parent.parent.GetComponent<Tool>();
            parentTool.SubSelected(true);
        }
    }
    public void SubSelected(bool subselected)
    {
        transform.GetChild(1).GetComponent<SelectionSquare>().SubSelected(subselected);
    }

    private void Update()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Visible", selected || Hovered());
    }

    public bool Hovered()
    {
        return GetComponent<MouseOverUI>().over && !GameManager.Instance.Menu.activeSelf;
    }
}