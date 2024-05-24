using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public partial class AnchorController : EntityController, IResettable
{
    [Separator] [InitializationField] [Required] public Transform AttachmentContainer;
    [InitializationField] [Required] public SyncTransform AttachmentContainerSyncTransform;
    [InitializationField] [Required] public Animator Animator;
    [InitializationField] [Required] public AnchorAttachFade AttachFade;
    
    [MyBox.ReadOnly] public int SortingLayerID;
    [MyBox.ReadOnly] public int OrderInLayer;
    
    [HideInInspector] public List<Transform> Balls = new();
    public LinkedList<AnchorBlock> Blocks = new();
    
    [HideInInspector] public SetSpeedBlock.Unit SpeedUnit;
    [HideInInspector] public SetRotationBlock.Unit RotationSpeedUnit;
    [HideInInspector] public float SpeedInput;
    [HideInInspector] public float RotationInput;
    [HideInInspector] public bool IsClockwise;
    public LinkedListNode<AnchorBlock> LoopBlockNode;
    public TweenerCore<Quaternion, Vector3, QuaternionOptions> RotationTween;
    [HideInInspector] public Ease Ease;
    
    public Vector2 StartPosition { get; private set; }
    private Quaternion startRotation;
    
    public AnchorBlock CurrentExecutingBlock;
    public LinkedListNode<AnchorBlock> CurrentExecutingNode;
    
    public Coroutine WaitCoroutine;
    
    public List<AnchorAttachment> Attachments;
    
    [HideInInspector] public Rigidbody2D Rb;
    private SpriteRenderer spriteRenderer;
    private EntityDragDrop entityDragDrop;
    private static readonly int editingString = Animator.StringToHash("Editing");
    private static readonly int playingString = Animator.StringToHash("Playing");
    
    public int LoopBlockIndex { get; set; } = -1;
    
    public bool IsSelected => AnchorManager.Instance.SelectedAnchor == this;
    public bool IsAttaching => AnchorAttachManager.Instance.InAttachMode && IsSelected;
    
    public override EditMode EditMode => EditModeManager.Anchor;
    
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
        Animator.SetBool(editingString, LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated);
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
        
        PlayManager.Instance.OnSwitchToPlay += AttachFade.FadeIn;
        PlayManager.Instance.OnSwitchToEdit += AttachFade.FadeOut;
        
        ((IResettable)this).Subscribe();
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
        
        if (CurrentExecutingNode == null)
        {
            // arrived at end of chain
            JumpToLoopIndex();
        }
        else
        {
            // execute next block
            CurrentExecutingBlock = CurrentExecutingNode.Value;
            CurrentExecutingBlock.Execute();
        }
    }
    
    private void JumpToLoopIndex()
    {
        // jump to block AFTER loop index, if existent
        if (LoopBlockNode == null) return;
        
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
        
        if (!hasActiveBlockAfter) return;
        
        CurrentExecutingBlock = CurrentExecutingNode!.Value;
        
        CurrentExecutingBlock.Execute();
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
            && LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated) SetLinesActive(true);
    }
    
    private void UpdateStartValues()
    {
        Transform t = transform;
        
        StartPosition = t.position;
        startRotation = t.rotation;
        LoopBlockNode = null;
    }
    
    public override void Delete() => AnchorManager.Remove(this);
    
    public override Data GetData() => new AnchorData(this);
    
    private void OnDestroy()
    {
        transform.DOKill();
        this.DOKill();
        spriteRenderer.DOKill();
        Rb.DOKill();
        
        PlayManager.Instance.OnSwitchToPlay -= AttachFade.FadeIn;
        PlayManager.Instance.OnSwitchToEdit -= AttachFade.FadeOut;
        
        ((IResettable)this).Unsubscribe();
    }
}