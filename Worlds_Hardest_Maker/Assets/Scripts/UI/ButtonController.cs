using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ButtonController : MonoBehaviour
{
    [FormerlySerializedAs("background")] public GameObject Background;

    [FormerlySerializedAs("backgroundPanel")]
    public RectTransform BackgroundPanel;

    [FormerlySerializedAs("PlaySound")] [FormerlySerializedAs("playSound")]
    public bool DoPlaySound;

    public void PlaySound()
    {
        if (AudioManager.Instance != null && DoPlaySound) AudioManager.Instance.Play("Click");
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
        Rect rect = rt.rect;
        float width = rect.width;
        float height = rect.height;

        float size = width < height ? width : height;

        float lineSize = size * 0.036f;

        BackgroundLineSize lineSizeController = Background.GetComponent<BackgroundLineSize>();
        lineSizeController.SetLineSize(lineSize);

        float backgroundPanelOffset = size * 0.065f;
        BackgroundPanel.offsetMin = new(backgroundPanelOffset, -backgroundPanelOffset);
        BackgroundPanel.offsetMax = new(backgroundPanelOffset, -backgroundPanelOffset);
    }

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public static void UpdateEVERYFUCKINGShit()
    {
        ButtonController[] buttons = Resources.FindObjectsOfTypeAll<ButtonController>();
        foreach (ButtonController controller in buttons)
        {
            controller.UpdateSomeShit();
        }
    }
}