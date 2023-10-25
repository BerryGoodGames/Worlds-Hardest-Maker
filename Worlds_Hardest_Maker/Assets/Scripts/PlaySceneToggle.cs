using MyBox;
using UnityEngine;

[ExecuteInEditMode]
public class PlaySceneToggle : MonoBehaviour
{
    private static PlaySceneToggle Instance { get; set; }

    [SerializeField] private bool isEdit;

    [ReadOnly] [OverrideLabel("Is Scene currently edit scene")]
    public bool IsEditPrev;

    // [ButtonMethod]
    public void UpdateStates()
    {
        ConditionalObject[] objArr = FindObjectsOfType<ConditionalObject>(true);

        foreach (ConditionalObject obj in objArr)
        {
            if (isEdit)
            {
                obj.IsActiveInEdit = obj.gameObject.activeSelf;
            }
            else
            {
                obj.IsActiveInPlay = obj.gameObject.activeSelf;
            }
        }
    }

    [ButtonMethod]
    public void Apply()
    {
        ConditionalObject[] objArr = FindObjectsOfType<ConditionalObject>(true);

        foreach (ConditionalObject obj in objArr)
        {
            if (isEdit != IsEditPrev)
            {
                if (isEdit)
                {
                    obj.IsActiveInPlay = obj.gameObject.activeSelf;
                }
                else
                {
                    obj.IsActiveInEdit = obj.gameObject.activeSelf;
                }
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