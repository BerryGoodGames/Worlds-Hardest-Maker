using UnityEngine;
using VContainer;

public class ToolbarManager : MonoBehaviour
{
    public static GameObject[] Tools;

    private EventBus eventBus;

    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChangeEvent);
    }

    private void OnEditModeChangeEvent(EditModeChangeEvent evt)
    {
        // update toolbarContainer
        GameObject[] tools = Tools;
        foreach (GameObject tool in tools)
        {
            Tool t = tool.GetComponent<Tool>();
            if (t.ToolEditMode == evt.NewEditMode)
            {
                // avoid recursion
                t.SwitchGameMode(false);
            }
        }
    }

    private void Start()
    {
        Tools = GameObject.FindGameObjectsWithTag("Tool");
    }

    public static void DeselectAll()
    {
        foreach (GameObject t in Tools)
        {
            Tool tool = t.GetComponent<Tool>();
            tool.SetSelected(false);
        }
    }

    private void OnDestroy()
    {
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChangeEvent);
    }
}