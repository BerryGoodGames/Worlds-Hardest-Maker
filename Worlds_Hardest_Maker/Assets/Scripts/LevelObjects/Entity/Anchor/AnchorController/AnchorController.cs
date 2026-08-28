using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public partial class AnchorController : EntityController, IResettable, IAnchorBlockExecutionContext
{
    [Separator] [InitializationField] [MustBeAssigned] public Transform AttachmentContainer;
    [InitializationField] [MustBeAssigned] public SyncTransform AttachmentContainerSyncTransform;
    [InitializationField] [MustBeAssigned] public Animator Animator;
    [InitializationField] [MustBeAssigned] public AnchorAttachFade AttachFade;
    
    [ReadOnly] public int SortingLayerID;
    [ReadOnly] public int OrderInLayer;
    
    [HideInInspector] public List<Transform> Balls = new();
    public LinkedList<AnchorBlock> Blocks = new();
    public LinkedListNode<AnchorBlock> LoopBlockNode;

    public Component TweenComponent => this;
    public MonoBehaviour CoroutineRunner => this;
    public Transform Transform => transform;
    public float ZAngle => transform.eulerAngles.z;
    public MovementUnit SpeedUnit { get; set; }
    public float SpeedInput { get; set; }
    public RotationUnit RotationUnit { get; set; }
    public float RotationInput { get; set; }
    public bool IsClockwise { get; set; }
    public Tween RotationTween { get; set; }
    public Ease Ease { get; set; }

    public Coroutine WaitCoroutine { get; set; }
    
    public Vector2 StartPosition { get; private set; }
    private Quaternion startRotation;
    
    public AnchorBlock CurrentExecutingBlock;
    public LinkedListNode<AnchorBlock> CurrentExecutingNode;
    
    // TODO: extract into registry
    private readonly List<AnchorAttachment> attachments;
    public IReadOnlyList<AnchorAttachment> Attachments => attachments;
    public void RegisterAttachment(AnchorAttachment a) => attachments.Add(a);
    public void UnregisterAttachment(AnchorAttachment a) => attachments.Remove(a);
    
    [HideInInspector] public Rigidbody2D Rb;
    private SpriteRenderer spriteRenderer;
    private EntityDragDrop entityDragDrop;
    private static readonly int editingString = Animator.StringToHash("Editing");
    private static readonly int playingString = Animator.StringToHash("Playing");

    [Inject] private IObjectResolver diContainer;
    private EventBus eventBus;
    [Inject] private IDrawService drawService;
    
    public int LoopBlockIndex { get; set; } = -1;
    
    public bool IsSelected => AnchorManager.Instance.SelectedAnchor == this;
    public bool IsAttaching => AnchorAttachManager.Instance.InAttachMode && IsSelected;
    
    public override EditMode EditMode => EditModeManager.Anchor;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => AttachFade.FadeIn();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => AttachFade.FadeIn();
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityDragDrop = GetComponent<EntityDragDrop>();
        
        SortingLayerID = spriteRenderer.sortingLayerID;
        OrderInLayer = spriteRenderer.sortingOrder;
        
        SpeedInput = 7;
        RotationInput = 360;
        Ease = Ease.Linear;
        
        if (!LevelSessionManager.Instance.IsEdit) return;
        Animator.SetBool(editingString, LevelSessionEditManager.Instance.CurrentEditMode.IsAnchorRelated);
    }
    
    protected override void Start()
    {
        base.Start();
        
        // update when moved by user
        entityDragDrop.OnMove += (oldPos, newPos) =>
        {
            MoveAnchor();
            StartCoroutine(Delayed());
            return;
            
            IEnumerator Delayed()
            {
                yield return new WaitForEndOfFrame();
                
                Attachments.ForEach(e => e.Controller.OnAnchorMove(oldPos, newPos));
            }
        };
        
        if (LevelSessionManager.Instance.IsEdit) UpdateStartValues();
        
        ((IResettable)this).Subscribe(eventBus);
    }
    
    public void AppendBlock(AnchorBlock block) => Blocks.AddLast(block);
    
    private void MoveAnchor()
    {
        // assuming that EntityDragDrop already moved transform
        
        StartPosition = transform.position;
        
        if (IsSelected) RenderLines();
    }
    
    #region Execution
    
    public void StartExecuting()
    {
        UpdateStartValues();
        
        CurrentExecutingNode = Blocks.First;
        CurrentExecutingBlock = CurrentExecutingNode.Value;
        CurrentExecutingBlock.Execute(this);
    }
    
    public void FinishCurrentExecution()
    {
        if (CurrentExecutingNode == null)
        {
            Debug.LogWarning("There was a FinishCurrentExecution() call, although there is no execution to finish");
            return;
        }
        
        CurrentExecutingNode = CurrentExecutingNode.Next;
        
        if (CurrentExecutingNode == null)
        {
            // arrived at end of chain
            CurrentExecutingBlock = null;
            JumpToLoopIndex();
        }
        else
        {
            // execute next block
            CurrentExecutingBlock = CurrentExecutingNode.Value;
            CurrentExecutingBlock.Execute(this);
        }
    }
    
    private void JumpToLoopIndex()
    {
        // jump to block AFTER loop index, if existant
        if (LoopBlockNode == null)
        {
            CurrentExecutingBlock = null;
            return;
        }
    
        CurrentExecutingNode = LoopBlockNode.Next;
        
        // only execute if there is a non-passive block after loop block
        LinkedListNode<AnchorBlock> currentNode = CurrentExecutingNode;
        bool hasActiveBlockAfter = false;
        while (currentNode != null)
        {
            if (currentNode.Value is IDurationBlock)
            {
                hasActiveBlockAfter = true;
                break;
            }
            currentNode = currentNode.Next;
        }
    
        if (!hasActiveBlockAfter)
        {
            CurrentExecutingBlock = null;
            CurrentExecutingNode = null;
            return;
        }
    
        CurrentExecutingBlock = CurrentExecutingNode!.Value;
        CurrentExecutingBlock.Execute(this);
    }

    public void StoreCurrentLoopIndex() =>
        // for some reason, LoopBlock can't access its node in the list Blocks
        // so the anchor has to store the index himself
        LoopBlockNode = CurrentExecutingNode;
    
    public void ResetExecution()
    {
        Transform t = transform;
        
        RotationTween.Kill();
        t.DOKill();
        t.SetPositionAndRotation(StartPosition, startRotation);
        
        if (WaitCoroutine != null) StopCoroutine(WaitCoroutine);
        
        CurrentExecutingBlock = null;
    }
    
    #endregion
    
    public void MergeToLayer()
    {
        const int LAYER_OFFSET = AnchorAttachment.INTERNAL_LAYER_OFFSET;
        
        int orderInLayer = Math.Min(OrderInLayer, LAYER_OFFSET - 1);
        
        int mergedOrder = Array.IndexOf(LayerManager.Instance.AllSortingLayerIDs, SortingLayerID) * LAYER_OFFSET + orderInLayer;
        
        spriteRenderer.sortingLayerName = LayerManager.Instance.SortingLayers.AnchorAbove;
        spriteRenderer.sortingOrder = mergedOrder;
    }
    
    public void ResetLayer()
    {
        spriteRenderer.sortingLayerID = SortingLayerID;
        spriteRenderer.sortingOrder = OrderInLayer;
    }
    
    public void ResetState()
    {
        ResetExecution();
        Animator.SetBool(playingString, false);
        
        if (AnchorManager.Instance.SelectedAnchor == this
            && LevelSessionEditManager.Instance.CurrentEditMode.IsAnchorRelated) SetLinesActive(true);
    }
    
    private void UpdateStartValues()
    {
        Transform t = transform;
        
        StartPosition = t.position;
        startRotation = t.rotation;
        LoopBlockNode = null;
    }
    
    public override void Delete() => AnchorManager.Instance.Remove(this);
    
    public override Data GetData() => new AnchorData(this);
    
    private void OnDestroy()
    {
        transform.DOKill();
        this.DOKill();
        spriteRenderer.DOKill();
        Rb.DOKill();
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        
        ((IResettable)this).Unsubscribe(eventBus);
    }
}