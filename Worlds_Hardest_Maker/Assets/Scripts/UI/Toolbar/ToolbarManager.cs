using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] tools;

    private void Start()
    {
        tools = GameObject.FindGameObjectsWithTag("Tool");
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