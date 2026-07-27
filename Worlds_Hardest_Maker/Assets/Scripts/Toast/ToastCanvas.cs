using MyBox;
using UnityEngine;

public class ToastCanvas : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform toastContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToastUI toastUIPrefab;
    
    public void InstantiateToast(ToastData toast)
    {
        ToastUI newToast = Instantiate(toastUIPrefab, Vector2.zero, Quaternion.identity, toastContainer);
        
        newToast.ApplyToastData(toast);
    }
}