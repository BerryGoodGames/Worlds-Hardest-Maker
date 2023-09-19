using UnityEngine;

public class QuickAccessOption : MonoBehaviour
{
    [SerializeField] private MouseOverUIRect mouseOverUIRect;
    [SerializeField] private AlphaTween tween;
    [Space] [SerializeField] private bool closesMenuOnClick;

    private Transform menu;
    private AlphaTween menuTween;

    private void Awake()
    {
        mouseOverUIRect.OnHovered += () => tween.SetVisible(true);
        mouseOverUIRect.OnUnhovered += () => tween.SetVisible(false);
    }

    private void Start()
    {
        menu = transform.parent;
        menuTween = menu.GetComponent<AlphaTween>();
    }

    public void CloseMenu()
    {
        if (!closesMenuOnClick) return;

        menuTween.SetVisible(false);
    }
}