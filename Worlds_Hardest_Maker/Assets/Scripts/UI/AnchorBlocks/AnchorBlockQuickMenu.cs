using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockQuickMenu : MonoBehaviour, IPointerClickHandler
{
    public bool Active = true;
    private AnchorBlockController anchorBlockController;

    private void Start() => anchorBlockController = GetComponent<AnchorBlockController>();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !Active) return;

        // open and position quick menu
        Vector2 mousePos = MouseManager.Instance.MouseCanvasPos;
        mousePos.y = MouseManager.Instance.MouseCanvasPos.y - GameManager.GetCanvasDimensions().y;

        AnchorBlockQuickMenuController quickMenu = ReferenceManager.Instance.AnchorBlockQuickMenu;

        quickMenu.SelectedAnchorBlock = anchorBlockController;
        quickMenu.RectTransform.anchoredPosition = mousePos;
        quickMenu.Tween.SetVisible(true);
    }
}