using UnityEngine;

public class QuickAccessOption : MonoBehaviour
{
    [SerializeField] private MouseOverUI mouseOverUI;
    [SerializeField] private AlphaUITween tween;
    [Space] [SerializeField] private bool closesMenuOnClick;

    private Transform menu;
    private AlphaUITween menuTween;

    private void Awake()
    {
        mouseOverUI.OnHovered += () => tween.SetVisible(true);
        mouseOverUI.OnUnhovered += () => tween.SetVisible(false);
    }

    private void Start()
    {
        menu = transform.parent;
        menuTween = menu.GetComponent<AlphaUITween>();
    }

    public void CloseMenu()
    {
        if (!closesMenuOnClick) return;

        menuTween.SetVisible(false);
    }
}