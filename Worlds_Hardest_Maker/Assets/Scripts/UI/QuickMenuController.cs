using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class QuickMenuController : MonoBehaviour
{
    private AlphaTween tween;
    private MouseOverUIRect mouseOver;

    private void Start()
    {
        tween = GetComponent<AlphaTween>();
        mouseOver = GetComponent<MouseOverUIRect>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) && !mouseOver.Over) tween.SetVisible(false);
    }
}