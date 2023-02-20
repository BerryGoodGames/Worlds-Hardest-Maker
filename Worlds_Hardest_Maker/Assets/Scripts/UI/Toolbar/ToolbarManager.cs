using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] Tools;

    private void Start()
    {
        Tools = GameObject.FindGameObjectsWithTag("Tool");
    }

    public static void DeselectAll()
    {
        foreach (GameObject t in Tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.IsSelected(false);
        }
    }
}