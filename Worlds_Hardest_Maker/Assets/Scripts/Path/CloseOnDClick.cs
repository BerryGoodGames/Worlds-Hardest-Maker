using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnDClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Animator windowAnimator;
    [SerializeField] private GameObject content;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)
        {
            windowAnimator.SetBool("closed", !windowAnimator.GetBool("closed"));
        }
    }
}
