using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] Tools;

    private void Start() => Tools = GameObject.FindGameObjectsWithTag("Tool");

    public static void DeselectAll()
    {
        foreach (GameObject t in Tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.SetSelected(false);
        }
    }

    public static void SelectEditMode(EditMode editMode)
    {
        // update toolbarContainer
        GameObject[] tools = Tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.ToolEditMode == editMode)
            {
                // avoid recursion
                t.SwitchGameMode(false);
            }
        }
    }
}