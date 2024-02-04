using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;
public class PlayerRecordingManager : MonoBehaviour
{
    [Separator("Settings")]
    [SerializeField] [PositiveValueOnly] private float recordingFrequency = 1; 
    [Space]
    [SerializeField] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), true)] private float displaySpeed = 4;
    [SerializeField] [ConditionalField(nameof(fixedDisplayDuration), false)] private float displayDuration = 2;

    [Header("Path")] [SerializeField] [InitializationField]  [OverrideLabel("Display at start")] private bool displayPath = true;
    
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

    private Coroutine recording;
    private Coroutine displaySpriteRecording;
    private Coroutine displayPathRecording;

    private List<Vector2> recordedPositions;
    private List<Vector2> recordedDeaths;

    private void Start()
    {
        recordingSpriteContainer.gameObject.SetActive(displaySprites);
        recordingPathContainer.gameObject.SetActive(displayPath);
         
        // on play: stop display coroutines, start recording
        PlayManager.Instance.OnSwitchToPlay += SwitchToPlay;
        PlayManager.Instance.OnPlaySceneSetup += SwitchToPlay;

        // on edit: stop recording, render path & sprites
        PlayManager.Instance.OnSwitchToEdit += () =>
        {
            if (recording != null) StopCoroutine(recording);

            if(recordingSpriteContainer.gameObject.activeSelf) displaySpriteRecording = RenderSpriteRecording();
            if(recordingPathContainer.gameObject.activeSelf) displayPathRecording = RenderPathRecording();
        };

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

    private IEnumerator RecordPlayer()
    {
        PlayerController player = PlayerManager.Instance.Player;

        if (player == null) yield break;

        recordedPositions = new();
        recordedDeaths = new();

        player.OnDeathEnter += () =>
        {
            if (LevelSessionEditManager.Instance.Editing) return;
            recordedDeaths.Add(player.transform.position);
        };

        // wait until player is out of the death animation
        while (player.InDeathAnim) yield return null;

        // save positions of player
        while (!LevelSessionEditManager.Instance.Editing)
        {
            // only record if player has moved
            if (recordedPositions.Count == 0 || (Vector2)player.transform.position != recordedPositions[^1])
            {
                recordedPositions.Add(player.transform.position);
            }

            yield return new WaitForSeconds(recordingFrequency);
        }
    }

    #region Display
    
    private Coroutine RenderSpriteRecording()
    {
        recordingSpriteContainer.DestroyChildren();
        return StartCoroutine(RenderLoop(i =>
        {
            // display player sprite
            float playerTrailIndex = (i - (recordedPositions.Count - (float)(spriteAmount * spriteFrequency))) / spriteFrequency + 1;

            if (!(playerTrailIndex > 0) || (recordedPositions.Count - 1 - i) % spriteFrequency != 0) return;
            
            SpriteRenderer playerTrail = Instantiate(playerSprite, recordedPositions[i], Quaternion.identity, recordingSpriteContainer);
            playerTrail.SetAlpha(playerTrailIndex / spriteAmount * spriteMaxAlpha);
        }));
    }

    private Coroutine RenderPathRecording()
    {
        recordingPathContainer.DestroyChildren();
        
        BeginNewLine();

        Quaternion rotation = Quaternion.Euler(0, 0, 45);
        
        int lineIndex = 0;

        return StartCoroutine(RenderLoop(i => {
            // display line
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineIndex, recordedPositions[i]);

            // if player dies there, begin new line (to avoid awkward teleportation lines)
            if (recordedDeaths.Contains(recordedPositions[i]))
            {
                Instantiate(recordingDeathPrefab, recordedPositions[i], rotation, recordingPathContainer);

                BeginNewLine();
                lineIndex = -1;
            }

            lineIndex++;
        }));
    }

    private IEnumerator RenderLoop(Action<int> action)
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;

        float displayDelay = fixedDisplayDuration 
            ? displayDuration / recordedPositions.Count
            : recordingFrequency / displaySpeed;
        
        for (int i = 0; i < recordedPositions.Count; i++)
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
}