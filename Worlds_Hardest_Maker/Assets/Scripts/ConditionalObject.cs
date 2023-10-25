using MyBox;
using UnityEngine;

public class ConditionalObject : MonoBehaviour
{
    public bool EditOnly;

    [ReadOnly] public bool IsActiveInEdit = true;
    [ReadOnly] public bool IsActiveInPlay = true;
}