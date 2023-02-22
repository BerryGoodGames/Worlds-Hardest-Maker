using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockQuickMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool active = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !active) return;

        // open and position quick menu
        Vector2 mousePos = MouseManager.Instance.MouseCanvasPos;
        mousePos.y = MouseManager.Instance.MouseCanvasPos.y - GameManager.GetCanvasDimensions().y;

        print(mousePos);

        ReferenceManager.Instance.AnchorBlockQuickMenu.anchoredPosition = mousePos;
        ReferenceManager.Instance.AnchorBlockQuickMenuTween.SetVisible(true);
    }
}
