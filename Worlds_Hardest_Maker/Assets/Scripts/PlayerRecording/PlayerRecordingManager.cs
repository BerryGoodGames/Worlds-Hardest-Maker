using LuLib.Transform;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using VContainer;

// TODO: serialize using nested structs
// TODO: split into RecordingController, RecordingRenderer, RecordingVisibilityController
// TODO: cache builtin enumerators
public class PlayerRecordingManager : MonoBehaviour
{
    public static PlayerRecordingManager Instance { get; private set; }
    
    [Separator("Settings")] [SerializeField] [PositiveValueOnly] private float recordingFrequency = 1;
    [Space] [SerializeField] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), true)] private float displaySpeed = 4;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), false)] private float displayDuration = 2;
    
    [Header("Path")] [SerializeField] [InitializationField] [OverrideLabel("Display at start")] private bool displayPath = true;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color deathColor = Color.red;
    [SerializeField] [OverrideLabel("Min Value")] [Range(0, 1)] private float minDeathColorValue;
    
    [Header("Sprite")] [SerializeField] [InitializationField] [OverrideLabel("Display at start")] private bool displaySprites = true;
    [SerializeField] [OverrideLabel("Frequency")] private uint spriteFrequency = 2;
    [SerializeField] [OverrideLabel("Max Alpha")] [Range(0, 1)] private float spriteMaxAlpha = 0.5f;
    [SerializeField] [OverrideLabel("Amount")] private uint spriteAmount = 9;
    
    [Separator("References")] [SerializeField] [InitializationField] [Required] private Transform recordingSpriteContainer;
    [SerializeField] [InitializationField] [Required] private Transform recordingPathContainer;
    [SerializeField] [InitializationField] [Required] private SpriteRenderer playerSprite;
    [SerializeField] [InitializationField] [Required] private LineRenderer recordingLinePrefab;
    [SerializeField] [InitializationField] [Required] private GameObject recordingDeathPrefab;
    
    [HideInInspector] public bool IsReplaying;
    
    private PlayerRecorder playerRecorder;

    private PlayerPathRenderer pathRenderer;
    private PlayerGhostRenderer ghostRenderer;
    
    private RecordingAnalyzer analyzer;
    
    private RecordingRenderLoop renderLoop;
    
    private Coroutine recording;
    private Coroutine displaySpriteRecording;
    private Coroutine displayPathRecording;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
        
        // on play: stop display coroutines, start recording
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
        
        // on edit: stop recording, render path & sprites
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        
        eventBus.Subscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Subscribe<ReplayEvent>(OnReplay);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        playerRecorder = new(recordingFrequency);
        
        pathRenderer = new(playerRecorder, recordingPathContainer, successColor, deathColor, minDeathColorValue, recordingDeathPrefab, recordingLinePrefab);
        ghostRenderer = new(playerRecorder, recordingSpriteContainer, spriteFrequency, spriteMaxAlpha, spriteAmount, playerSprite);
        
        analyzer = new();
        
        renderLoop = new(fixedDisplayDuration, displayDuration, displaySpeed, recordingFrequency);
    }
    
    private void Start()
    {
        recordingSpriteContainer.gameObject.SetActive(displaySprites);
        recordingPathContainer.gameObject.SetActive(displayPath);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => OnSwitchToPlay();
    private void OnSwitchToPlay(SetupPlaySceneEvent evt) => OnSwitchToPlay();
    private void OnSwitchToPlay()
    {
        if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
        if (displayPathRecording != null) StopCoroutine(displayPathRecording);
        
        if (recordingSpriteContainer != null) recordingSpriteContainer.DestroyChildren();
        if (recordingPathContainer != null) recordingPathContainer.DestroyChildren();
        
        recording = StartCoroutine(playerRecorder.RecordPlayer());
    }
    
    private void OnSwitchToEdit(SwitchToEditEvent evt) => AnalyzeAndRender();
    
    private void AnalyzeAndRender()
    {
        if (recording != null) StopCoroutine(recording);

        analyzer.AnalyzeFrames(playerRecorder);

        if (recordingSpriteContainer.gameObject.activeSelf)
        {
            displaySpriteRecording = StartCoroutine(ghostRenderer.RenderSpriteRecording(renderLoop));
        }
        if (recordingPathContainer.gameObject.activeSelf)
        {
            displayPathRecording = StartCoroutine(pathRenderer.RenderPathRecording(renderLoop));
        }
    }
    
    public void StartPlayerRecording()
    {
        if (recording != null) StopCoroutine(recording);
        
        recording = StartCoroutine(playerRecorder.RecordPlayer());
    }
    
    public void ToggleSpriteVisibility() => SetSpriteVisible(!recordingSpriteContainer.gameObject.activeSelf);
    
    public void SetSpriteVisible(bool visible)
    {
        recordingSpriteContainer.gameObject.SetActive(visible);
        
        if (visible) displaySpriteRecording = StartCoroutine(ghostRenderer.RenderSpriteRecording(renderLoop));
        else
        {
            if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
            recordingSpriteContainer.DestroyChildren();
        }
    }
    
    public void TogglePathVisibility() => SetPathVisible(!recordingPathContainer.gameObject.activeSelf);
    
    public void SetPathVisible(bool visible)
    {
        recordingPathContainer.gameObject.SetActive(visible);
        
        if (displayPathRecording != null) StopCoroutine(displayPathRecording);
        
        recordingPathContainer.DestroyChildren();
        
        if (visible) displayPathRecording = StartCoroutine(pathRenderer.RenderPathRecording(renderLoop));
    }
    
    private void OnPlayAgain(PlayAgainEvent evt)
    {
        SetSpriteVisible(false);
        SetPathVisible(false);
        
        StartPlayerRecording();
        
        IsReplaying = false;
    }
    
    private void OnReplay(ReplayEvent evt)
    {
        IsReplaying = true;
        
        SetSpriteVisible(false);
        SetPathVisible(true);
    }
    
    private void OnDestroy()
    {
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SetupPlaySceneEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<PlayAgainEvent>(OnPlayAgain);
        eventBus.Unsubscribe<ReplayEvent>(OnReplay);
    }
}