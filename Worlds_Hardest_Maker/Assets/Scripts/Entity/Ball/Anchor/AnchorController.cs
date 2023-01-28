using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnchorController : MonoBehaviour
{
    [SerializeField] private ChildrenOpacity ballContainerChildrenOpacity;
    public Animator animator;

    [HideInInspector] public List<GameObject> balls = new();
    public LinkedList<AnchorBlock> blocks = new();

    [HideInInspector] public bool applySpeed = true;
    private bool startApplySpeed;
    [HideInInspector] public bool applyAngularSpeed = true;
    private bool startApplyAngularSpeed;

    [HideInInspector] public float speed;
    private float startSpeed;
    [HideInInspector] public float angularSpeed;
    private float startAngularSpeed;

    [HideInInspector] public Ease ease;
    private Ease startEase;

    private Vector2 startPosition;
    private Quaternion startRotation;

    public AnchorBlock currentExecutingBlock;

    [HideInInspector] public Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        speed = 7;
        angularSpeed = 360;
        ease = Ease.Linear;

        UpdateStartValues();
    }

    private void AddBlocks(AnchorBlock block)
    {
        blocks.AddLast(block);
    }

    public void StartExecuting()
    {
        UpdateStartValues();

        Testing();

        currentExecutingBlock = blocks.First.Value;
        currentExecutingBlock.Execute();
    }

    public void FinishCurrentExecution()
    {
        if (currentExecutingBlock == null)
        {
            Debug.LogWarning("There was a FinishCurrentExecution() call, although there is no execution to finish");
            return;
        }

        LinkedListNode<AnchorBlock> thisBlockNode = blocks.Find(currentExecutingBlock);
        if (thisBlockNode == null)
            throw new Exception(
                "Couldn't find block currently executing in block list of anchor (this error should be impossible)");

        currentExecutingBlock = thisBlockNode.Next?.Value;
        currentExecutingBlock?.Execute();
    }

    public void Testing()
    {
        AddBlocks(new SetSpeedBlock(this, 1.5f, SetSpeedBlock.Unit.TIME));
        AddBlocks(new MoveToBlock(this, 0, 0));
        AddBlocks(new MoveToBlock(this, 1, 3));
        AddBlocks(new MoveToBlock(this, 2, -1));
        AddBlocks(new GoToBlock(this, 0));
    }

    public void ResetExecution()
    {
        Transform t = transform;

        rb.DOKill();
        t.DOKill();
        currentExecutingBlock = null;

        rb.position = startPosition;
        t.rotation = startRotation;
        speed = startSpeed;
        applySpeed = startApplySpeed;
        angularSpeed = startAngularSpeed;
        applyAngularSpeed = startApplyAngularSpeed;
        ease = startEase;
    }

    private void UpdateStartValues()
    {
        startPosition = rb.position;
        startRotation = transform.rotation;
        startSpeed = speed;
        startApplySpeed = applySpeed;
        startAngularSpeed = angularSpeed;
        startApplyAngularSpeed = applyAngularSpeed;
        startEase = ease;
    }
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

    public void BallFadeIn(float endOpacity, float time)
    {
        StartCoroutine(ballContainerChildrenOpacity.FadeIn(endOpacity, time));
    }
}