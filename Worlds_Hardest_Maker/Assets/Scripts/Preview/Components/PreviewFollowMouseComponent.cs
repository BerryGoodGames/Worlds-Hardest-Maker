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
        
        if (hasFollowMouseComp)
        {
            followMouseComp.WorldPosition = currentEditMode.WorldPositionType;
        }
    }
}