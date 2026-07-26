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
        // disable following mouse when user is panning
        bool isPanning = KeyBinds.GetKeyBind("Camera_Pan");
        bool shouldEnable = !isPanning;
        
        if (hasFollowMouseComp && followMouseComp.AllowSnapToNewCell != shouldEnable)
        {
            followMouseComp.AllowSnapToNewCell = shouldEnable;
        }
        
        // tell follow mouse correct behaviour
        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        
        if (hasFollowMouseComp)
        {
            followMouseComp.WorldPosition = currentEditMode.WorldPositionType;
        }
    }
}