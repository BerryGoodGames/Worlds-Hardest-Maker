using System;
using LuLib.Vector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NavigationManager : MonoBehaviour
{
    private EventSystem system;

    private void Update()
    {
        // check if it should navigate
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        // check if it can navigate
        if (system.currentSelectedGameObject == null ||
            !system.currentSelectedGameObject.TryGetComponent(out Selectable selectable)) return;

        Selectable next;

        // try to navigate down
        next = selectable.FindSelectableOnDown();

        if (next != null)
        {
            // try to navigate left again
            next = next.FindSelectableOnLeft();

            if (next == null)
            {
                next = selectable.FindSelectableOnDown();
            }
        }
        else
        {
            // try to navigate right
            next = selectable.FindSelectableOnRight();

            if (next == null) return;
        }

        // if it's an input field, also set the text caret
        if (next.TryGetComponent(out InputField inputField))
        {
            inputField.OnPointerClick(new(system));  
        }

        // select new game object
        system.SetSelectedGameObject(next.gameObject, new(system));
    }

    private void Start()
    {
        system = EventSystem.current;
    }
}
