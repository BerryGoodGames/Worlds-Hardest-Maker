using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockQuickMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AnchorBlockController anchorBlockController;
    [SerializeField] private bool active = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !active) return;

        // open and position quick menu
        Vector2 mousePos = MouseManager.Instance.MouseCanvasPos;
        print(MouseManager.Instance.MouseCanvasPos);
        mousePos.y = MouseManager.Instance.MouseCanvasPos.y - GameManager.GetCanvasDimensions().y;

        // TODO: fix position

        AnchorBlockQuickMenuController quickMenu = ReferenceManager.Instance.AnchorBlockQuickMenu;

        quickMenu.SelectedAnchorBlock = anchorBlockController;
        quickMenu.RectTransform.anchoredPosition = mousePos;
        quickMenu.Tween.SetVisible(true);
    }
}