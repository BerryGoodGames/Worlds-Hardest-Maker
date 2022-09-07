using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public GameObject background;
    public RectTransform backgroundPanel;

    public bool playSound = false;

    public void PlaySound()
    {
        if (AudioManager.Instance != null && playSound) AudioManager.Instance.Play("Click");
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
        ButtonController[] buttons = Resources.FindObjectsOfTypeAll<ButtonController>();
        foreach(ButtonController controller in buttons)
        {
            controller.UpdateSomeShit();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonController))]
public class ButtonControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonController script = (ButtonController)target;
        if (GUILayout.Button("Update outline / background panel size"))
        {
            script.UpdateSomeShit();
        }

        if(GUILayout.Button("Update EVERY button"))
        {
            ButtonController.UpdateEVERYFUCKINGShit();
        }
    }
}
#endif