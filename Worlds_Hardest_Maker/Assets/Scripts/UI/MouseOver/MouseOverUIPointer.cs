using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Detects mouse hovering of a UI element using pointer events
/// </summary>
public class MouseOverUIPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Over { get; private set; }

    private Action OnHovered = () => { };
    private Action OnUnhovered = () => { };

    [SerializeField] private bool printDbg;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Over)
        {
            OnHovered();

            if (printDbg) print($"{name} (Pointer): Pointer enter");
        }

        Over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Over)
        {
            OnUnhovered();

            if (printDbg) print($"{name} (Pointer): Pointer exit");
        }

        Over = false;
    }
}