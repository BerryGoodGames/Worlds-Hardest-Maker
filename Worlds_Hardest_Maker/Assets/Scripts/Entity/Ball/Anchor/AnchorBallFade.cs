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

    // public void BallFadeOut(AnimationEvent animationEvent)
    // {
    //     float endOpacity = animationEvent.floatParameter;
    //     if (float.TryParse(animationEvent.stringParameter, out float time))
    //         StartCoroutine(container.FadeOut(endOpacity, time));
    // }
    //
    // public void BallFadeOut(float endOpacity, float time) => StartCoroutine(container.FadeOut(endOpacity, time));
    //
    // public void BallFadeIn(AnimationEvent animationEvent)
    // {
    //     float endOpacity = animationEvent.floatParameter;
    //     if (float.TryParse(animationEvent.stringParameter, out float time))
    //         BallFadeIn(endOpacity, time);
    // }
    //
    // public void BallFadeIn(float endOpacity, float time) => StartCoroutine(container.FadeIn(endOpacity, time));

    public void BallFadeOut() => container.FadeTo(fadeOutOpacity, fadeDuration);

    public void BallFadeIn() => container.FadeTo(fadeInOpacity, fadeDuration);

}