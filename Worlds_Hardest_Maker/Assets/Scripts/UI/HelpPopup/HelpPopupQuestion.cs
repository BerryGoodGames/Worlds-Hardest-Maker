using MyBox;
using UnityEngine;

public class HelpPopupQuestion : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private HelpPopup popup;

    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform popupContainer;

    public void OnButtonClick()
    {
        Instantiate(popup, popupContainer);
    }
}
