using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CButton : MonoBehaviour
{
    public GameObject background;
    public RectTransform backgroundPanel;

    public bool playSound = false;

    public void PlaySound()
    {
        if (MAudio.Instance != null && playSound) MAudio.Instance.Play("Click");
    }

    public void Deselect()
    {
        if (EventSystem.current.currentSelectedGameObject.Equals(gameObject))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void UpdateSomeShit()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;

        float size = width < height ? width : height;

        float lineSize = size * 0.036f;

        BackgroundLineSize lineSizeController = background.GetComponent<BackgroundLineSize>();
        lineSizeController.SetLineSize(lineSize);

        float bpoffset = size * 0.065f;
        backgroundPanel.offsetMin = new(bpoffset, -bpoffset);
        backgroundPanel.offsetMax = new(bpoffset, -bpoffset);
    }

    public static void UpdateEVERYFUCKINGShit()
    {
        CButton[] buttons = Resources.FindObjectsOfTypeAll<CButton>();
        foreach(CButton controller in buttons)
        {
            controller.UpdateSomeShit();
        }
    }
}