using System.Collections;
using MyBox;
using UnityEngine;

public abstract class ChainableTween : TweenController
{
    [Separator("Chain settings")] [SerializeField]
    private bool hasNext;

    [SerializeField] [ConditionalField(nameof(hasNext))] [PositiveValueOnly]
    private float nextDelay;

    [SerializeField] [ConditionalField(nameof(hasNext))]
    private ChainableTween next;

    public abstract void StartChain();

    protected IEnumerator StartDelay()
    {
        if (!hasNext) yield break;

        yield return new WaitForSecondsRealtime(nextDelay + Delay);

        next.StartChain();
    }
}

public abstract class TweenController : MonoBehaviour
{
    [Separator("Tween Settings")] [PositiveValueOnly]
    public float Duration;

    [PositiveValueOnly] public float Delay;
}