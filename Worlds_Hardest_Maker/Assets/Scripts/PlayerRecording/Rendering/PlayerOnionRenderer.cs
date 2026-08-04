using System;
using System.Collections;
using System.Collections.Generic;
using LuLib.Transform;
using MyBox;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PlayerOnionRenderer<T> where T : IRecordingFrame
{
    private IRecordingFrameStorage<T> frameStorage;
    [SerializeField] private Transform recordingSpriteContainer;
    [SerializeField] [PositiveValueOnly] private int frequency = 2;
    [SerializeField] [Range(0, 1)] private float maxAlpha = 0.5f;
    [SerializeField] [PositiveValueOnly] private int count = 9;
    [SerializeField] private SpriteRenderer playerPrefabSprite;

    public void SetFrameStorage(IRecordingFrameStorage<T> frameStorage)
    {
        this.frameStorage = frameStorage;
    }

    public void SetActive(bool active)
    {
        recordingSpriteContainer.gameObject.SetActive(active);
    }

    public bool IsActive()
    {
        return recordingSpriteContainer.gameObject.activeSelf;
    }

    public void Clear()
    {
        recordingSpriteContainer.DestroyChildren();
    }
    
    public IEnumerator RenderSpriteRecording(RecordingRenderLoop renderLoop)
    {
        if(frameStorage == null)
        {
            Debug.LogWarning("Frame storage not assigned.");
            yield break;
        }
        
        IReadOnlyList<T> frames = frameStorage.Frames;

        if (frames == null)
        {
            Debug.LogWarning("Recorded positions not found.");
            yield break;
        }
        
        recordingSpriteContainer.DestroyChildren();
        
        int startIndex = Mathf.Max(frames.Count - count * frequency, 0);

        yield return renderLoop.Play(
            (positions, i) =>
            {
                // display player sprite
                float playerTrailIndex = (i - (positions.Count - (float)(count * frequency))) /
                    frequency + 1;

                if (playerTrailIndex <= 0 || (positions.Count - 1 - i) % frequency != 0) return;

                SpriteRenderer playerTrail = Object.Instantiate(
                    playerPrefabSprite, positions[i].Position, Quaternion.identity, recordingSpriteContainer
                );

                float alpha = playerTrailIndex * maxAlpha / count;
                playerTrail.SetAlpha(alpha);
            }, frames, startIndex
        );
    }
}