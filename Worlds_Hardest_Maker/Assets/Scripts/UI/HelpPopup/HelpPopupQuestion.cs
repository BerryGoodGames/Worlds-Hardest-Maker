using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class HelpPopupQuestion : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private HelpPopup popup;
    
    [SerializeField] [InitializationField] [Required] private RectTransform popupContainer;
    
    public void OnButtonClick() => Instantiate(popup, popupContainer);
}