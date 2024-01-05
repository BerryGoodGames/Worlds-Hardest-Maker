using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorBlockQuickMenuTrigger : MonoBehaviour, IPointerClickHandler
{
    public bool Active = true;
    private AnchorBlockController anchorBlockController;

    private void Start() => anchorBlockController = GetComponent<AnchorBlockController>();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !Active) return;

        ReferenceManager.Instance.AnchorBlockQuickMenu.Activate(anchorBlockController);
    }
}