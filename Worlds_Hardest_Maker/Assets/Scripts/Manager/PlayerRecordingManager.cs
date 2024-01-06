using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;

public class PlayerRecordingManager : MonoBehaviour
{
    [Separator("Settings")] [SerializeField] private float recordingFrequency = 1f;
    [SerializeField] private float displayDelay = 0.25f;
    [Header("Player")] [SerializeField] private uint playerTrailFrequency = 4;
    [SerializeField] [Range(0, 1)] private float playerTrailMaxAlpha = 0.75f;
    [SerializeField] private uint playerTrailAmount = 5;

    [Separator("References")] [SerializeField] [InitializationField] [MustBeAssigned] private Transform recordingContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer playerSprite;

    private LineRenderer lineRenderer;

    private Coroutine recording;

    private List<Vector2> recordedPositions;

    private void Awake() => lineRenderer = recordingContainer.GetComponent<LineRenderer>();

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += () =>
        {
            ClearRecording();
            recording = StartCoroutine(RecordPlayer());
        };

        PlayManager.Instance.OnSwitchToEdit += () =>
        {
            if (recording != null) StopCoroutine(recording);

            StartCoroutine(DisplayRecording(recordedPositions));
        };
    }

    private IEnumerator RecordPlayer()
    {
        PlayerController player = PlayerManager.Instance.Player;
        
        if(player == null) yield break;

        recordedPositions = new();

        // save positions of player
        while (!EditModeManagerOther.Instance.Editing)
        {
            recordedPositions.Add(player.transform.position);

            yield return new WaitForSeconds(recordingFrequency);
        }
    }

    private IEnumerator DisplayRecording(IReadOnlyList<Vector2> recordedPositions)
    {
        if (recordedPositions == null) yield break;

        for (int i = 0; i < recordedPositions.Count; i++)
        {
            // display line
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i, recordedPositions[i]);

            // display player sprite
            float playerTrailIndex = (i - (recordedPositions.Count - (float)(playerTrailAmount * playerTrailFrequency))) / playerTrailFrequency + 1;

            if (playerTrailIndex > 0 && (recordedPositions.Count - 1 - i) % playerTrailFrequency == 0)
            {
                SpriteRenderer playerTrail = Instantiate(playerSprite, recordedPositions[i], Quaternion.identity, recordingContainer);

                playerTrail.SetAlpha(playerTrailIndex / playerTrailAmount * playerTrailMaxAlpha);
            }

            // wait delay
            yield return new WaitForSeconds(displayDelay);
        }
    }

    private void ClearRecording()
    {
        recordingContainer.DestroyChildren();

        lineRenderer.positionCount = 0;
    }

    public void ToggleVisibility() => recordingContainer.gameObject.SetActive(!recordingContainer.gameObject.activeSelf);
}