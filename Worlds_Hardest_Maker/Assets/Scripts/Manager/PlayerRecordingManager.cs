using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using MyBox;
using NaughtyAttributes;
using UnityEngine;

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
    
    public event Action<Vector2> OnPathRenderUpdate = _ => { };
    public event Action OnFinishReplay = () => { };
    
    [HideInInspector] public bool IsReplaying;
    
    private LineRenderer lineRenderer;
    private Color lineColor;
    private const float valueShift = 0.3090169945f;
    
    private Coroutine recording;
    private Coroutine displaySpriteRecording;
    private Coroutine displayPathRecording;
    
    private List<Recording> recordedPositions;
    
    private void Start()
    {
        recordingSpriteContainer.gameObject.SetActive(displaySprites);
        recordingPathContainer.gameObject.SetActive(displayPath);
        
        // on play: stop display coroutines, start recording
        PlayManager.Instance.OnSwitchToPlay += SwitchToPlay;
        PlayManager.Instance.OnPlaySceneSetup += SwitchToPlay;
        
        // on edit: stop recording, render path & sprites
        PlayManager.Instance.OnSwitchToEdit += RenderRecording;
        
        LevelCompleteManager.Instance.OnPlayAgain += OnPlayAgain;
        LevelCompleteManager.Instance.OnReplay += OnReplay;
    }
    
    private void SwitchToPlay()
    {
        if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
        if (displayPathRecording != null) StopCoroutine(displayPathRecording);
        
        recordingSpriteContainer.DestroyChildren();
        recordingPathContainer.DestroyChildren();
        
        recording = StartCoroutine(RecordPlayer());
    }
    
    private void RenderRecording()
    {
        if (recordedPositions == null) return;
        
        if (recording != null) StopCoroutine(recording);
        
        // mark successful runs
        bool successful = true;
        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            if (successful && recordedPositions[i].Died && i != recordedPositions.Count - 1)
            {
                recordedPositions[i].StartSuccessfulLine = true;
                successful = false;
            }
            
            if (recordedPositions[i].CheckpointHit)
            {
                if (successful) recordedPositions[i].StartSuccessfulLine = true;
                
                successful = true;
            }
            
            if (i == 0 && successful) recordedPositions[i].StartSuccessfulLine = true;
        }
        
        if (recordingSpriteContainer.gameObject.activeSelf) displaySpriteRecording = RenderSpriteRecording();
        if (recordingPathContainer.gameObject.activeSelf) displayPathRecording = RenderPathRecording();
    }
    
    public void StartPlayerRecording()
    {
        if (recording != null) StopCoroutine(recording);
        
        recording = StartCoroutine(RecordPlayer());
    }
    
    public IEnumerator RecordPlayer()
    {
        if (PlayerManager.Instance.Player == null) yield return new WaitForEndOfFrame();
        if (PlayerManager.Instance.Player == null) yield break;
        
        PlayerController player = PlayerManager.Instance.Player;
        
        recordedPositions = new();
        
        player.OnDeathEnd += RecordDeath;
        player.OnCheckpointEnter += RecordCheckpoint;
        
        // wait until player is out of the death animation
        while (player.InDeathAnim) yield return null;
        
        // save positions of player
        while (!LevelSessionEditManager.Instance.Editing)
        {
            // only record if player has moved
            if (recordedPositions.Count == 0 || (Vector2)player.transform.position != recordedPositions[^1].Position)
                recordedPositions.Add(new(player.transform.position));
            
            yield return new WaitForSeconds(recordingFrequency);
        }
        
        player.OnDeathEnd -= RecordDeath;
        player.OnCheckpointEnter -= RecordCheckpoint;
        yield break;
        
        void RecordDeath()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            recordedPositions.Add(new(player.transform.position, true));
        }
        
        void RecordCheckpoint()
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            recordedPositions.Add(new(player.transform.position, checkpointHit: true));
        }
    }
    
    #region Display
    
    private Coroutine RenderSpriteRecording()
    {
        if (recordedPositions == null) return null;
        
        recordingSpriteContainer.DestroyChildren();
        
        int startIndex = (int)Mathf.Max(recordedPositions.Count - spriteAmount * spriteFrequency, 0);
        
        return StartCoroutine(
            RenderLoop(
                i =>
                {
                    // display player sprite
                    float playerTrailIndex = (i - (recordedPositions.Count - (float)(spriteAmount * spriteFrequency))) / spriteFrequency + 1;
                    
                    if (playerTrailIndex <= 0 || (recordedPositions.Count - 1 - i) % spriteFrequency != 0) return;
                    
                    SpriteRenderer playerTrail = Instantiate(
                        playerSprite, recordedPositions[i].Position, Quaternion.identity, recordingSpriteContainer
                    );
                    
                    playerTrail.SetAlpha(playerTrailIndex / spriteAmount * spriteMaxAlpha);
                }, startIndex
            )
        );
    }
    
    private Coroutine RenderPathRecording()
    {
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();
        
        lineRenderer.startColor = deathColor;
        lineRenderer.endColor = deathColor;
        
        Quaternion rotation = Quaternion.Euler(0, 0, 45);
        
        return StartCoroutine(
            RenderLoop(
                i =>
                {
                    // display line
                    AddLinePosition(recordedPositions[i].Position);
                    
                    // if player dies or hits checkpoint and then will die, begin new red line 
                    if (recordedPositions[i].Died ||
                        (recordedPositions[i].CheckpointHit && !recordedPositions[i].StartSuccessfulLine))
                    {
                        if (recordedPositions[i].Died)
                            Instantiate(recordingDeathPrefab, recordedPositions[i].Position, rotation, recordingPathContainer);
                        
                        // calculate new color
                        float value = 1;
                        
                        if (minDeathColorValue < 1)
                            value = (lineRenderer.startColor.GetHSV().z + valueShift) % (1 - minDeathColorValue) + minDeathColorValue;
                        
                        Color newColor = Color.red.SetValue(value);
                        
                        newColor.a = deathColor.a;
                        
                        BeginNewLine();
                        
                        lineRenderer.startColor = newColor;
                        lineRenderer.endColor = newColor;
                        
                        if (recordedPositions[i].CheckpointHit) AddLinePosition(recordedPositions[i].Position);
                    }
                    
                    // change color to green when successful run starts
                    if (recordedPositions[i].StartSuccessfulLine && !recordedPositions[i].CheckpointHit)
                    {
                        BeginNewLine();
                        
                        lineRenderer.startColor = successColor;
                        lineRenderer.endColor = successColor;
                    }
                    
                    OnPathRenderUpdate.Invoke(recordedPositions[i].Position);
                    
                    if (IsReplaying && i == recordedPositions.Count - 1) OnFinishReplay.Invoke();
                }
            )
        );
    }
    
    private void AddLinePosition(Vector2 position)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }
    
    private IEnumerator RenderLoop(Action<int> action, int startIndex = 0)
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;
        
        float displayDelay = fixedDisplayDuration
            ? displayDuration / recordedPositions.Count
            : recordingFrequency / displaySpeed;
        
        for (int i = startIndex; i < recordedPositions.Count; i++)
        {
            action.Invoke(i);
            
            // wait delay
            yield return new WaitForSeconds(displayDelay);
        }
    }
    
    private void BeginNewLine() => lineRenderer = Instantiate(recordingLinePrefab, recordingPathContainer);
    
    #endregion
    
    public void ToggleSpriteVisibility() => SetSpriteVisible(!recordingSpriteContainer.gameObject.activeSelf);
    
    public void SetSpriteVisible(bool visible)
    {
        recordingSpriteContainer.gameObject.SetActive(visible);
        
        if (visible) displaySpriteRecording = RenderSpriteRecording();
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
        
        if (visible) displayPathRecording = RenderPathRecording();
    }
    
    private void OnPlayAgain()
    {
        SetSpriteVisible(false);
        SetPathVisible(false);
        
        StartPlayerRecording();
        
        IsReplaying = false;
    }
    
    private void OnReplay()
    {
        IsReplaying = true;
        
        SetSpriteVisible(false);
        SetPathVisible(true);
    }
    
    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToPlay -= SwitchToPlay;
        PlayManager.Instance.OnPlaySceneSetup -= SwitchToPlay;
        PlayManager.Instance.OnSwitchToEdit -= RenderRecording;
        LevelCompleteManager.Instance.OnPlayAgain -= OnPlayAgain;
        LevelCompleteManager.Instance.OnReplay -= OnReplay;
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private class Recording
    {
        public readonly Vector2 Position;
        public readonly bool Died;
        public readonly bool CheckpointHit;
        public bool StartSuccessfulLine;
        
        public Recording(Vector2 position, bool died = false, bool checkpointHit = false)
        {
            Position = position;
            Died = died;
            CheckpointHit = checkpointHit;
        }
        
        public override string ToString() =>
            $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}, start successful line: {StartSuccessfulLine}}}";
    }
}