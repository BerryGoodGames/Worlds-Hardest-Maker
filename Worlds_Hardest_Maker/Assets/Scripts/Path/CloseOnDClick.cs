using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnDClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Animator windowAnimator;

    private static readonly int closed = Animator.StringToHash("Closed");

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            windowAnimator.SetBool(closed, !windowAnimator.GetBool(closed));
        }
    }
}