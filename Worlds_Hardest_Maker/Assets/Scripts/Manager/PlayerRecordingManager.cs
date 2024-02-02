using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRecordingManager : MonoBehaviour
{
    [Separator("Settings")] 
    
    [Header("Line")]
    [SerializeField] [OverrideLabel("Display")] private bool displayPathLine = true;
    [SerializeField] [ConditionalField(nameof(displayPathLine))] private float recordingFrequency = 1;
    [SerializeField] [ConditionalField(nameof(displayPathLine))] private float displaySpeed = 4;
    
    [Header("Sprite")] 
    [SerializeField] [OverrideLabel("Frequency")] private uint spriteFrequency = 2;
    [SerializeField] [OverrideLabel("Max Alpha")] [Range(0, 1)] private float spriteMaxAlpha = 0.5f;
    [SerializeField] [OverrideLabel("Amount")] private uint spriteAmount = 9;
    

    [Separator("References")] 
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform recordingContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer playerSprite;

    private LineRenderer lineRenderer;

    private Coroutine recording;
    private Coroutine displayRecording;

    private List<Vector2> recordedPositions;

    private void Awake() => lineRenderer = recordingContainer.GetComponent<LineRenderer>();

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            if(displayRecording != null) StopCoroutine(displayRecording);
            
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
        
        if(player == null) yield break;
        
        recordedPositions = new();

        // wait until player is out of the death animation
        while(player.InDeathAnim) yield return null;
        
        // save positions of player
        while (!LevelSessionEditManager.Instance.Editing)
        {
            recordedPositions.Add(player.transform.position);

            yield return new WaitForSeconds(recordingFrequency);
        }
    }

    private IEnumerator DisplayRecording(IReadOnlyList<Vector2> recordedPositions)
    {
        if (recordedPositions.IsNullOrEmpty()) yield break;

        float displayDelay = recordingFrequency / displaySpeed;
        
        for (int i = 0; i < recordedPositions.Count; i++)
        {
            if (displayPathLine)
            {
                // display line
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(i, recordedPositions[i]);
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

    private void ClearRecordingDisplay()
    {
        recordingContainer.DestroyChildren();

        lineRenderer.positionCount = 0;
    }

    public void ToggleVisibility() => recordingContainer.gameObject.SetActive(!recordingContainer.gameObject.activeSelf);
}