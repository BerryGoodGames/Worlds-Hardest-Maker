using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Color;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRecordingManager : MonoBehaviour
{
    [Separator("Settings")]
    [SerializeField] [PositiveValueOnly] private float recordingFrequency = 1; 
    [Space]
    [SerializeField] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), true)] private float displaySpeed = 4;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), false)] private float displayDuration = 2;

    [Header("Path")] 
    [SerializeField] [InitializationField]  [OverrideLabel("Display at start")] private bool displayPath = true;
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color deathColor = Color.red;
    [SerializeField] [OverrideLabel("Min Value")] [Range(0, 1)] private float minDeathColorValue;
    
    [Header("Sprite")] [SerializeField] [InitializationField]  [OverrideLabel("Display at start")] private bool displaySprites = true;
    [SerializeField] [OverrideLabel("Frequency")] private uint spriteFrequency = 2;
    [SerializeField] [OverrideLabel("Max Alpha")] [Range(0, 1)] private float spriteMaxAlpha = 0.5f;
    [SerializeField] [OverrideLabel("Amount")] private uint spriteAmount = 9;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private Transform recordingSpriteContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform recordingPathContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer playerSprite;
    [SerializeField] [InitializationField] [MustBeAssigned] private LineRenderer recordingLinePrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject recordingDeathPrefab;

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

        // when in play mode, display path recording when player wins
        PlayerManager.Instance.OnWin += () =>
        {
            if (!LevelSessionManager.Instance.IsEdit)
            {
                recordingPathContainer.gameObject.SetActive(true);
                RenderPathRecording();
            }
        };

        return;
        
        void SwitchToPlay()
        {
            if (displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
            if (displayPathRecording != null) StopCoroutine(displayPathRecording);

            recordingSpriteContainer.DestroyChildren();
            recordingPathContainer.DestroyChildren();
            
            recording = StartCoroutine(RecordPlayer());
        }
    }

    private void RenderRecording()
    {
        if (recording != null) StopCoroutine(recording);

        // mark successful runs
        bool successful = true;
        for (int i = recordedPositions.Count - 1; i >= 0; i--)
        {
            if (successful && recordedPositions[i].Died && i != recordedPositions.Count - 1)
            {
                recordedPositions[i + 1].StartSuccessfulLine = true;
                successful = false;
            }
            
            if (recordedPositions[i].CheckpointHit)
            {
                if (successful) recordedPositions[i].StartSuccessfulLine = true;
                
                successful = true;
            }

            if (i == 0 && successful) recordedPositions[i].StartSuccessfulLine = true;
        }
        
        // print(recordedPositions.Count);
        // foreach (Recording recordedPosition in recordedPositions)
        // {
        //     print(recordedPosition);
        // }
        
        if (recordingSpriteContainer.gameObject.activeSelf) displaySpriteRecording = RenderSpriteRecording();
        if (recordingPathContainer.gameObject.activeSelf) displayPathRecording = RenderPathRecording();
    }

    private IEnumerator RecordPlayer()
    {
        PlayerController player = PlayerManager.Instance.Player;

        if (player == null) yield break;

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
            {
                recordedPositions.Add(new(player.transform.position));
            }

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
            recordedPositions.Add(new(player.transform.position, checkpointHit:true));
        }
    }

    #region Display
    
    private Coroutine RenderSpriteRecording()
    {
        recordingSpriteContainer.DestroyChildren();
        
        int startIndex = (int)Mathf.Max(recordedPositions.Count - spriteAmount * spriteFrequency, 0);
        
        return StartCoroutine(RenderLoop(i =>
        {
            // display player sprite
            float playerTrailIndex = (i - (recordedPositions.Count - (float)(spriteAmount * spriteFrequency))) / spriteFrequency + 1;

            if (!(playerTrailIndex > 0) || (recordedPositions.Count - 1 - i) % spriteFrequency != 0) return;
            
            SpriteRenderer playerTrail = Instantiate(playerSprite, recordedPositions[i].Position, Quaternion.identity, recordingSpriteContainer);
            playerTrail.SetAlpha(playerTrailIndex / spriteAmount * spriteMaxAlpha);
        }, startIndex));
    }

    private Coroutine RenderPathRecording()
    {
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();
        
        lineRenderer.startColor = deathColor;
        lineRenderer.endColor = deathColor;

        Quaternion rotation = Quaternion.Euler(0, 0, 45);
        
        int lineIndex = 0;

        return StartCoroutine(RenderLoop(i => {
            // display line
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineIndex, recordedPositions[i].Position);

            // if player dies or hits checkpoint and then will die, begin new red line 
            if (recordedPositions[i].Died ||
                (recordedPositions[i].CheckpointHit && !recordedPositions[i].StartSuccessfulLine))
            {
                if(recordedPositions[i].Died)
                    Instantiate(recordingDeathPrefab, recordedPositions[i].Position, rotation, recordingPathContainer);

                // calculate new color
                float value = 1;

                if (minDeathColorValue < 1)
                {
                    value = lineRenderer.startColor.GetHSV().z;
                    
                    value += valueShift;
                    value %= 1 - minDeathColorValue;
                    value += minDeathColorValue;
                }
                
                Color newColor = Color.red.SetValue(value);

                newColor.a = deathColor.a;
                
                BeginNewLine();
                lineIndex = -1;

                lineRenderer.startColor = newColor;
                lineRenderer.endColor = newColor;
            }
            
            // change color to green when successful run starts
            if (recordedPositions[i].StartSuccessfulLine)
            {
                BeginNewLine();
                lineIndex = -1;

                lineRenderer.startColor = successColor;
                lineRenderer.endColor = successColor;
            }

            lineIndex++;
        }));
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
    
    private void BeginNewLine()
    {
        lineRenderer = Instantiate(recordingLinePrefab, recordingPathContainer);
    }
    
    #endregion

    public void ToggleSpriteVisibility()
    {
        recordingSpriteContainer.gameObject.SetActive(!recordingSpriteContainer.gameObject.activeSelf);

        if (recordingSpriteContainer.gameObject.activeSelf) displaySpriteRecording = RenderSpriteRecording();
        else
        {
            if(displaySpriteRecording != null) StopCoroutine(displaySpriteRecording);
            recordingSpriteContainer.DestroyChildren();
        }
    }

    public void TogglePathVisibility()
    {
        recordingPathContainer.gameObject.SetActive(!recordingPathContainer.gameObject.activeSelf);

        if (recordingPathContainer.gameObject.activeSelf) displayPathRecording = RenderPathRecording();
        else
        {
            if(displayPathRecording != null) StopCoroutine(displayPathRecording);
            recordingPathContainer.DestroyChildren();
        }
    }

    private class Recording
    {
        public readonly Vector2 Position;
        public readonly bool Died;
        public readonly bool CheckpointHit;
        public bool StartSuccessfulLine = false;

        public Recording(Vector2 position, bool died = false, bool checkpointHit = false)
        {
            Position = position;
            Died = died;
            CheckpointHit = checkpointHit;
        }

        public override string ToString()
        {
            return $"{{position: {Position}, died: {Died}, checkpoint hit: {CheckpointHit}, start successful line: {StartSuccessfulLine}}}";
        }
    }
}