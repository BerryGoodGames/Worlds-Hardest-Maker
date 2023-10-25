using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ConditionalObject : MonoBehaviour
{
    public bool EditOnly;
    
    [ReadOnly] public bool IsActiveInEdit = true;
    [ReadOnly] public bool IsActiveInPlay = true;

    [SerializeField] private UnityEvent whenPlay;
    [SerializeField] private UnityEvent whenEdit;

    private void Start()
    {
        if(EditModeManager.Instance.Editing) whenEdit?.Invoke();
        else whenPlay?.Invoke();
    }
}