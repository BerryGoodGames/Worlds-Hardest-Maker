using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class QuickMenuController : MonoBehaviour
{
    private AlphaUITween tween;
    private MouseOverUIRect mouseOver;

    private void Start()
    {
        tween = GetComponent<AlphaUITween>();
        mouseOver = GetComponent<MouseOverUIRect>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) && !mouseOver.Over) tween.SetVisible(false);
    }
}