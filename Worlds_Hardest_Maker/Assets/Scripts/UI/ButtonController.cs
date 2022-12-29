using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    public GameObject background;
    public RectTransform backgroundPanel;

    public bool playSound;

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
        Rect rect = rt.rect;
        float width = rect.width;
        float height = rect.height;

        float size = width < height ? width : height;

        float lineSize = size * 0.036f;

        BackgroundLineSize lineSizeController = background.GetComponent<BackgroundLineSize>();
        lineSizeController.SetLineSize(lineSize);

        float backgroundPanelOffset = size * 0.065f;
        backgroundPanel.offsetMin = new(backgroundPanelOffset, -backgroundPanelOffset);
        backgroundPanel.offsetMax = new(backgroundPanelOffset, -backgroundPanelOffset);
    }

    public static void UpdateEVERYFUCKINGShit()
    {
        ButtonController[] buttons = Resources.FindObjectsOfTypeAll<ButtonController>();
        foreach (ButtonController controller in buttons)
        {
            controller.UpdateSomeShit();
        }
    }
}