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
        if (!Input.GetMouseButtonDown(1)) return;

        tween.SetVisible(false);
    }
}
