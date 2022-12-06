using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] Tools;

    private void Start()
    {
        Tools = GameObject.FindGameObjectsWithTag("Tool");

        // edit mode wall when starting
        Tool firstTool = transform.GetChild(0).GetComponent<Tool>();
        firstTool.SwitchGameMode();
    }

    public static void DeselectAll()
    {
        foreach (GameObject t in Tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.Selected(false);
        }
    }
}