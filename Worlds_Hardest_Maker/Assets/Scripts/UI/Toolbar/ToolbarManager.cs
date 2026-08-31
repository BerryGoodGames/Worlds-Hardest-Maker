using MyBox;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    [Tag] [SerializeField] [InitializationField] private string toolTag;
    
    private static GameObject[] tools;

    private void Start()
    {
        tools = GameObject.FindGameObjectsWithTag(toolTag);
    }

    public static void DeselectAll()
    {
        foreach (GameObject t in tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.SetSelected(false);
        }
    }
}