using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public GameObject Background;
    public RectTransform BackgroundPanel;
    public bool DoPlaySound;

    public void PlaySound()
    {
        if (AudioManager.Instance != null && DoPlaySound) AudioManager.Instance.Play("Click");
    }

    public void Deselect()
    {
        if (EventSystem.current.currentSelectedGameObject.Equals(gameObject))
            EventSystem.current.SetSelectedGameObject(null);
    }

    [ButtonMethod]
    public void UpdateOutlineAndBackgroundPanelSize()
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
    [ButtonMethod]
    public static void UpdateEVERYButton()
    {
        ButtonController[] buttons = Resources.FindObjectsOfTypeAll<ButtonController>();
        foreach (ButtonController controller in buttons)
        {
            controller.UpdateOutlineAndBackgroundPanelSize();
        }
    }
}