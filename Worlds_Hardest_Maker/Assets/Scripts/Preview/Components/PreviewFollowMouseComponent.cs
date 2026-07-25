using UnityEngine;

public class PreviewFollowMouseComponent : MonoBehaviour
{
    private FollowMouse followMouseComp;
    private bool hasFollowMouseComp;
    
    private void Start()
    {
        hasFollowMouseComp = TryGetComponent(out followMouseComp);
    }
    
    private void Update()
    {
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        
        if (!SelectionManager.Instance.Selecting && hasFollowMouseComp)
        {
            followMouseComp.WorldPosition = currentEditMode.Attributes.IsField || currentEditMode == EditModeManager.Delete
                ? WorldPositionType.Matrix
                : WorldPositionType.Grid;
        }
    }
    
    public FollowMouse GetFollowMouseComponent()
    {
        return followMouseComp;
    }
}