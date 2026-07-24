using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class HelpPopupQuestion : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private HelpPopup popup;
    
    [SerializeField] [InitializationField] [Required] private RectTransform popupContainer;
    
    [Inject] private IObjectResolver diContainer;
    
    public void OnButtonClick()
    {
        HelpPopup instance = Instantiate(popup, popupContainer);
        
        diContainer.InjectGameObject(instance.gameObject);
    }
}