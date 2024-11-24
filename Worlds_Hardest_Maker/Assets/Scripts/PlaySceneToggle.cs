using JetBrains.Annotations;
using MyBox;
using UnityEngine;

[ExecuteInEditMode]
public class PlaySceneToggle : MonoBehaviour
{
    private static PlaySceneToggle Instance { get; set; }
    
    [SerializeField] private bool isEdit;
    
    [ReadOnly] [OverrideLabel("Is Scene currently edit scene")] public bool IsEditPrev;
    
    [ButtonMethod]
    public void UpdateStates()
    {
        ConditionalObject[] objArr = FindObjectsByType<ConditionalObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (ConditionalObject obj in objArr)
        {
            if (isEdit) obj.IsActiveInEdit = obj.gameObject.activeSelf;
            else obj.IsActiveInPlay = obj.gameObject.activeSelf;
        }
    }
    
    [ButtonMethod]
    [UsedImplicitly]
    public void Apply()
    {
        ConditionalObject[] objArr = FindObjectsByType<ConditionalObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (ConditionalObject obj in objArr)
        {
            if (isEdit != IsEditPrev)
            {
                if (isEdit) obj.IsActiveInPlay = obj.gameObject.activeSelf;
                else obj.IsActiveInEdit = obj.gameObject.activeSelf;
            }
            
            obj.gameObject.SetActive(isEdit ? obj.IsActiveInEdit : obj.IsActiveInPlay);
        }
        
        IsEditPrev = isEdit;
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}