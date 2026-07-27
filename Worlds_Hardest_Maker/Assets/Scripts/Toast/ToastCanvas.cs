using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ToastCanvas : MonoBehaviour
{
    [Inject] private IObjectResolver diContainer;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform toastContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastUI toastUIPrefab;
    
    public void InstantiateToast(ToastData toast)
    {
        ToastUI newToast = Instantiate(toastUIPrefab, Vector2.zero, Quaternion.identity, toastContainer);
        
        diContainer.InjectGameObject(newToast.gameObject);
        
        newToast.ApplyToastData(toast);
    }
}