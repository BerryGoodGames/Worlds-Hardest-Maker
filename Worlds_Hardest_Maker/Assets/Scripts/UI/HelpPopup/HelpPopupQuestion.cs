using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class HelpPopupQuestion : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private HelpPopup popup;
    
    [SerializeField] [InitializationField] [Required] private RectTransform popupContainer;
    
    [Inject] private IObjectResolver IObjectResolver;
    
    public void OnButtonClick()
    {
        HelpPopup instance = Instantiate(popup, popupContainer);
        
        IObjectResolver.InjectGameObject(instance.gameObject);
    }
}