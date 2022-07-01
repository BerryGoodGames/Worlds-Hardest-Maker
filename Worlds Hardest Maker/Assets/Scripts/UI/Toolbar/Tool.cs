using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public GameManager.EditMode toolName;
    [HideInInspector] public bool selected;
    public void SwitchGameMode()
    {
        ToolbarManager.DeselectAll();
        Selected(true);
        GameManager.Instance.CurrentEditMode = toolName;

    }
    public void Selected(bool selected)
    {
        transform.GetChild(1).GetComponent<SelectionSquare>().Selected(selected);

        this.selected = selected;
    }

    private void Update()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("Visible", selected || Hovered());
    }

    public bool Hovered()
    {
        return GetComponent<MouseOverUI>().over;
    }
}
