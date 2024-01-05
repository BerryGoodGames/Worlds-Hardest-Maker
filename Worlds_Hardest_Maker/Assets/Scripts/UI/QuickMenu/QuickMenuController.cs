using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class QuickMenuController : MonoBehaviour
{
    protected AlphaTween Tween;
    private MouseOverUIRect mouseOver;

    private void Start()
    {
        Tween = GetComponent<AlphaTween>();
        mouseOver = GetComponent<MouseOverUIRect>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) && !mouseOver.Over) Tween.SetVisible(false);
    }
}