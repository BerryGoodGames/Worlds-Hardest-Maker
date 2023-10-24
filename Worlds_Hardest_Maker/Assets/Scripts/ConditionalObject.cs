using MyBox;
using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    [ReadOnly] public bool IsActiveInEdit = true;
    [ReadOnly] public bool IsActiveInPlay = true;

    private void Start()
    {
        if ((IsActiveInEdit && !LevelSessionManager.Instance.IsEdit)
            || (IsActiveInPlay && LevelSessionManager.Instance.IsEdit))
        {
            Destroy(gameObject);
        }
    }
}