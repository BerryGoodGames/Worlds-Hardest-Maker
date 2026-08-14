using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class HelpPopupQuestion : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private HelpPopup popup;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform popupContainer;
    
    [Inject] private IObjectResolver diContainer;
    
    public void OnButtonClick()
    {
        HelpPopup instance = Instantiate(popup, popupContainer);
        
        diContainer.InjectGameObject(instance.gameObject);
    }
}