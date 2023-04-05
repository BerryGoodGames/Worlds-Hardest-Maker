using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class AnchorBlockQuickMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AnchorBlockController anchorBlockController;
    [FormerlySerializedAs("active")] public bool Active = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !Active) return;

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