using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PreviewSpriteComponent : MonoBehaviour
{
    [SerializeField] [MustBeAssigned] [InitializationField] private Sprite deleteSprite;
    [Space] [SerializeField] [Range(0, 255)] private float alpha;
    
    public bool ShowSpriteWhenPasting { get; set; }
    
    public SpriteRenderer SpriteRenderer { get; private set; }

    [ReadOnly] public bool CheckUpdateEveryFrame = true;
    
    private EventBus eventBus;
    private PreviewSpriteDataProvider previewSpriteDataProvider;
    
    [Inject]
    private void Construct(EventBus eventBus, PreviewSpriteDataProvider previewSpriteDataProvider)
    {
        this.eventBus = eventBus;
        this.previewSpriteDataProvider = previewSpriteDataProvider;
        
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<EditModeInitializedEvent>(OnEditModeInitialized);
    }

    public float GetAlpha() => alpha / 255f;
    public void SetAlpha(float newAlpha) => alpha = newAlpha * 255f;
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => UpdateSprite();
    private void OnEditModeInitialized(EditModeInitializedEvent evt) => UpdateSprite();
    private void OnEditModeChange(EditModeChangeEvent evt) => UpdateSprite();

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    ///     Updates sprite of preview to the current edit mode
    /// </summary>
    private void UpdateSprite()
    {
        SetSprite(LevelSessionEditManager.Instance.CurrentEditMode);
    }
    
    private void SetSprite(EditMode editMode)
    {
        float normalizedAlpha = alpha / 255f;
        PreviewSpriteData previewSpriteData = previewSpriteDataProvider.GetPreviewSpriteData(editMode, normalizedAlpha, forceShowPreviewSprite: ShowSpriteWhenPasting);
        
        ApplyPreviewData(previewSpriteData);
    }
    
    public void ApplyPreviewData(PreviewSpriteData previewSpriteData)
    {
        SpriteRenderer.sprite = previewSpriteData.IsDelete ? deleteSprite : previewSpriteData.Sprite;
        SpriteRenderer.color = previewSpriteData.Color;
        transform.localScale = previewSpriteData.Scale;
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EditModeInitializedEvent>(OnEditModeInitialized);
    }
}