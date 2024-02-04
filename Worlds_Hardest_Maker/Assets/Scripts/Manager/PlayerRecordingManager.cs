using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRecordingManager : MonoBehaviour
{
    [Separator("Settings")] [Header("Line")] [SerializeField] [OverrideLabel("Display")] private bool displayPathLine = true;
    [SerializeField] [ConditionalField(nameof(displayPathLine))] private float recordingFrequency = 1;
    
    [Header("Display")]
    [SerializeField] [ConditionalField(nameof(displayPathLine))] private bool fixedDisplayDuration;
    [SerializeField] [ConditionalField( new[]{ nameof(displayPathLine), nameof(fixedDisplayDuration), }, new[]{ false, true, })] private float displaySpeed = 4;
    [SerializeField] [ConditionalField( new[]{ nameof(displayPathLine), nameof(fixedDisplayDuration), }, new[]{ false, false, })] private float displayDuration = 2;

    [Header("Sprite")] [SerializeField] [OverrideLabel("Frequency")] private uint spriteFrequency = 2;
    [SerializeField] [OverrideLabel("Max Alpha")] [Range(0, 1)] private float spriteMaxAlpha = 0.5f;
    [SerializeField] [OverrideLabel("Amount")] private uint spriteAmount = 9;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private Transform recordingContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer playerSprite;
    [SerializeField] [InitializationField] [MustBeAssigned] private LineRenderer recordingLinePrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject recordingDeathPrefab;

    private LineRenderer lineRenderer;

    private Coroutine recording;
    private Coroutine displayRecording;

    private List<Vector2> recordedPositions;
    private List<Vector2> recordedDeaths;

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            if (displayRecording != null) StopCoroutine(displayRecording);

            ClearRecordingDisplay();
            recording = StartCoroutine(RecordPlayer());
        };

        PlayManager.Instance.OnSwitchToEdit += () =>
        {
            if (recording != null) StopCoroutine(recording);

            ClearRecordingDisplay();

            displayRecording = StartCoroutine(DisplayRecording(recordedPositions));
        };
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

    private IEnumerator DisplayRecording(IReadOnlyList<Vector2> recordedPositions)
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;

        float displayDelay = fixedDisplayDuration 
            ? displayDuration / recordedPositions.Count
            : recordingFrequency / displaySpeed;

        BeginNewLine();

        Quaternion rotation = Quaternion.Euler(0, 0, 45);
        
        int lineIndex = 0;
        for (int i = 0; i < recordedPositions.Count; i++, lineIndex++)
        {
            if (displayPathLine)
            {
                // display line
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineIndex, recordedPositions[i]);
            }
            
            // if player dies there, begin new line (to avoid awkward teleportation lines)
            if (recordedDeaths.Contains(recordedPositions[i]))
            {
                Instantiate(recordingDeathPrefab, recordedPositions[i], rotation, recordingContainer);
                
                BeginNewLine();
                lineIndex = -1;
            }

            // display player sprite
            float playerTrailIndex = (i - (recordedPositions.Count - (float)(spriteAmount * spriteFrequency))) / spriteFrequency + 1;

            if (playerTrailIndex > 0 && (recordedPositions.Count - 1 - i) % spriteFrequency == 0)
            {
                SpriteRenderer playerTrail = Instantiate(playerSprite, recordedPositions[i], Quaternion.identity, recordingContainer);

                playerTrail.SetAlpha(playerTrailIndex / spriteAmount * spriteMaxAlpha);
            }

            // wait delay
            yield return new WaitForSeconds(displayDelay);
        }
    }

    private void BeginNewLine()
    {
        lineRenderer = Instantiate(recordingLinePrefab, recordingContainer);
    }

    private void ClearRecordingDisplay()
    {
        recordingContainer.DestroyChildren();
    }

    public void ToggleVisibility() => recordingContainer.gameObject.SetActive(!recordingContainer.gameObject.activeSelf);
}