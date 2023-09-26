using MyBox;
using UnityEngine;

public class AnchorBallFade : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private ChildrenOpacity container;

    [Separator] [SerializeField] [InitializationField]
    private float fadeDuration = 0.2f;

    [SerializeField] [InitializationField] private float fadeInOpacity = 1;

    [SerializeField] [InitializationField] private float fadeOutOpacity = 0.3f;

    public void BallFadeOut() => container.FadeTo(fadeOutOpacity, fadeDuration);

    public void BallFadeIn() => container.FadeTo(fadeInOpacity, fadeDuration);

}