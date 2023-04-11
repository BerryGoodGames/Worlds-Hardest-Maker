using UnityEngine;

public class QuickMenuController : MonoBehaviour
{
    private AlphaUITween tween;

    private void Start()
    {
        tween = GetComponent<AlphaUITween>();
    }

    private void Update()
    {
        if (!(Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))) return;

        tween.SetVisible(false);
    }
}