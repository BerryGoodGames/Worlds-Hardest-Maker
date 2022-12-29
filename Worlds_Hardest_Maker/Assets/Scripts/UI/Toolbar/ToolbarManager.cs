using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] tools;

    private void Start()
    {
        tools = GameObject.FindGameObjectsWithTag("Tool");

        // edit mode wall when starting
        Tool firstTool = transform.GetChild(0).GetComponent<Tool>();
        firstTool.SwitchGameMode();
    }

    public static void DeselectAll()
    {
        foreach (GameObject t in tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.Selected(false);
        }
    }
}