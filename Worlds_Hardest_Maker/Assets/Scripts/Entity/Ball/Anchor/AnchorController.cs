using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public partial class AnchorController : Controller
{
    [SerializeField] [InitializationField] private ChildrenOpacity ballContainerChildrenOpacity;
    [InitializationField] public Transform BallContainer;
    [InitializationField] public Animator Animator;

    [HideInInspector] public List<Transform> Balls = new();
    public LinkedList<AnchorBlock> Blocks = new();

    [HideInInspector] public SetSpeedBlock.Unit SpeedUnit;
    [HideInInspector] public SetRotationBlock.Unit RotationSpeedUnit;
    [HideInInspector] public float TimeInput;
    [HideInInspector] public float RotationTimeInput;
    [HideInInspector] public bool IsClockwise;
    public TweenerCore<float, float, FloatOptions> RotationTween;
    [HideInInspector] public Ease Ease;

    private Vector2 startPosition;
    private Quaternion startRotation;

    public AnchorBlock CurrentExecutingBlock;
    public LinkedListNode<AnchorBlock> CurrentExecutingNode;


    [HideInInspector] public Rigidbody2D Rb;
    private SpriteRenderer spriteRenderer;
    private EntityDragDrop entityDragDrop;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityDragDrop = GetComponent<EntityDragDrop>();
    }

    private void Start()
    {
        // update when moved by user
        entityDragDrop.OnMove += (oldPosition, newPosition) => MoveAnchor(newPosition - oldPosition);

        TimeInput = 7;
        RotationTimeInput = 360;
        Ease = Ease.Linear;

        UpdateStartValues();

        RenderLines();
    }

    public void AppendBlock(AnchorBlock block) => Blocks.AddLast(block);

    private void MoveAnchor(Vector2 delta)
    {
        // assuming that EntityDragDrop already moved transform

        LinkedList<AnchorBlock> blocks = Blocks;

        // loop through data blocks and add offset
        foreach (AnchorBlock currentBlock in blocks)
        {
            if (currentBlock is PositionAnchorBlock positionBlock)
            {
                // add offset
                positionBlock.Target += delta;
            }
        }

        // loop through UI blocks and add offset
        ChainController mainChain = ReferenceManager.Instance.MainChainController;

        foreach (AnchorBlockController anchorBlockController in mainChain.Children)
        {
            if (anchorBlockController is not PositionAnchorBlockController positionBlock) continue;

            // add offset
            Vector2 currentValue = positionBlock.PositionInput.GetPositionValues();

            positionBlock.PositionInput.SetPositionValues(currentValue.x + delta.x, currentValue.y + delta.y);
        }
    }

    #region Execution

    public void StartExecuting()
    {
        UpdateStartValues();

        CurrentExecutingNode = Blocks.First;
        CurrentExecutingBlock = CurrentExecutingNode.Value;
        CurrentExecutingBlock.Execute();
    }

    public void FinishCurrentExecution()
    {
        if (CurrentExecutingBlock == null)
        {
            Debug.LogWarning("There was a FinishCurrentExecution() call, although there is no execution to finish");
            return;
        }

        CurrentExecutingNode = CurrentExecutingNode.Next;
        CurrentExecutingBlock = CurrentExecutingNode?.Value;
        CurrentExecutingBlock?.Execute();
    }

    public void ResetExecution()
    {
        Transform t = transform;

        Rb.DOKill();
        t.DOKill();
        CurrentExecutingBlock = null;

        Rb.position = startPosition;
        t.rotation = startRotation;
    }

    #endregion

    #region Ball fade

    public void BallFadeOut(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            StartCoroutine(ballContainerChildrenOpacity.FadeOut(endOpacity, time));
    }

    public void BallFadeIn(AnimationEvent animationEvent)
    {
        float endOpacity = animationEvent.floatParameter;
        if (float.TryParse(animationEvent.stringParameter, out float time))
            BallFadeIn(endOpacity, time);
    }

    public void BallFadeIn(float endOpacity, float time) =>
        StartCoroutine(ballContainerChildrenOpacity.FadeIn(endOpacity, time));

    #endregion

    private void UpdateStartValues()
    {
        startPosition = Rb.position;
        startRotation = transform.rotation;
    }

    public override Data GetData() => new AnchorData(this);
}