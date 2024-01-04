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
        EditModeManagerOther.Instance.OnPlay += () =>
        {
            ClearRecording();
            recording = StartCoroutine(RecordPlayer());
        };

        EditModeManagerOther.Instance.OnEdit += () =>
        {
            if (recording != null) StopCoroutine(recording);

            StartCoroutine(DisplayRecording(recordedPositions));
        };
    }

    private IEnumerator RecordPlayer()
    {
        Transform player = PlayerManager.GetPlayer()?.transform;

        recordedPositions = new();

        if (player is null) yield break;

        while (!EditModeManagerOther.Instance.Editing)
        {
            recordedPositions.Add(player.position);

            yield return new WaitForSeconds(recordingFrequency);
        }
    }

    private IEnumerator DisplayRecording(IReadOnlyList<Vector2> recordedPositions)
    {
        if (recordedPositions == null) yield break;

        for (int i = 0; i < recordedPositions.Count; i++)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i, recordedPositions[i]);

            float playerTrailIndex = (i - (recordedPositions.Count - (float)(playerTrailAmount * playerTrailFrequency))) / playerTrailFrequency + 1;

            if (playerTrailIndex > 0 && i % playerTrailFrequency == 0)
            {
                SpriteRenderer playerTrail = Instantiate(playerSprite, recordedPositions[i], Quaternion.identity, recordingContainer);

                playerTrail.SetAlpha(playerTrailIndex / playerTrailAmount * playerTrailMaxAlpha);
            }

            yield return new WaitForSeconds(displayDelay);
        }
    }

    private void ClearRecording()
    {
        recordingContainer.DestroyChildren();

        lineRenderer.positionCount = 0;
    }
}