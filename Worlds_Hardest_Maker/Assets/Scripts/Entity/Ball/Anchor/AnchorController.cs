using System;
using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.Serialization;

public class AnchorController : Controller
{
    [SerializeField] private ChildrenOpacity ballContainerChildrenOpacity;

    [FormerlySerializedAs("ballContainer")]
    public Transform BallContainer;

    [FormerlySerializedAs("animator")] public Animator Animator;

    [FormerlySerializedAs("balls")] [HideInInspector]
    public List<GameObject> Balls = new();

    public LinkedList<AnchorBlock> Blocks = new();

    [FormerlySerializedAs("applySpeed")] [HideInInspector]
    public bool ApplySpeed = true;

    private bool startApplySpeed;

    [FormerlySerializedAs("applyAngularSpeed")] [HideInInspector]
    public bool ApplyAngularSpeed = true;

    private bool startApplyAngularSpeed;

    [FormerlySerializedAs("speed")] [HideInInspector]
    public float Speed;

    private float startSpeed;

    [FormerlySerializedAs("angularSpeed")] [HideInInspector]
    public float AngularSpeed;

    private float startAngularSpeed;

    [FormerlySerializedAs("ease")] [HideInInspector]
    public Ease Ease;

    private Ease startEase;

    private Vector2 startPosition;
    private Quaternion startRotation;

    public AnchorBlock CurrentExecutingBlock;

    [FormerlySerializedAs("rb")] [HideInInspector]
    public Rigidbody2D Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();

        Speed = 7;
        AngularSpeed = 360;
        Ease = Ease.Linear;

        UpdateStartValues();
    }

    public void AppendBlock(AnchorBlock block)
    {
        Blocks.AddLast(block);
    } 

    public void StartExecuting()
    {
        UpdateStartValues();
        
        CurrentExecutingBlock = Blocks.First.Value;
        CurrentExecutingBlock.Execute();
    }

    public void FinishCurrentExecution()
    {
        if (CurrentExecutingBlock == null)
        {
            Debug.LogWarning("There was a FinishCurrentExecution() call, although there is no execution to finish");
            return;
        }

        LinkedListNode<AnchorBlock> thisBlockNode = Blocks.Find(CurrentExecutingBlock);
        if (thisBlockNode == null)
            throw new(
                "Couldn't find block currently executing in block list of anchor (this error should be impossible)");
        
        CurrentExecutingBlock = thisBlockNode.Next?.Value;
        CurrentExecutingBlock?.Execute();
    }

    public void Testing()
    {
        AppendBlock(new SetSpeedBlock(this, 1.5f, SetSpeedBlock.Unit.TIME));
        AppendBlock(new SetAngularSpeedBlock(this, 3f, SetAngularSpeedBlock.Unit.TIME));
        AppendBlock(new MoveToBlock(this, 0, 0));
        AppendBlock(new RotateBlock(this, 1));
        AppendBlock(new MoveToBlock(this, 1, 3));
        AppendBlock(new MoveToBlock(this, 2, -1));
        AppendBlock(new GoToBlock(this, 0));
    }

    public void ResetExecution()
    {
        Transform t = transform;

        Rb.DOKill();
        t.DOKill();
        CurrentExecutingBlock = null;

        Rb.position = startPosition;
        t.rotation = startRotation;
        Speed = startSpeed;
        ApplySpeed = startApplySpeed;
        AngularSpeed = startAngularSpeed;
        ApplyAngularSpeed = startApplyAngularSpeed;
        Ease = startEase;
    }

    private void UpdateStartValues()
    {
        startPosition = Rb.position;
        startRotation = transform.rotation;
        startSpeed = Speed;
        startApplySpeed = ApplySpeed;
        startAngularSpeed = AngularSpeed;
        startApplyAngularSpeed = ApplyAngularSpeed;
        startEase = Ease;
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

    public override Data GetData()
    {
        throw new NotImplementedException();
    }
}