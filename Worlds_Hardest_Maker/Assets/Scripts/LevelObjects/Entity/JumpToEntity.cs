using System;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using UnityEngine;

/// <summary>
///     Jumps to specified transform Target
/// </summary>
public class JumpToEntity : MonoBehaviour
{
    private readonly Dictionary<string, (GameObject target, Renderer targetRenderer)> targetList = new();

    [Space] public bool Smooth;

    [ConditionalField(nameof(Smooth))] [MinValue(0.001f)] public float Time;

    [Space] [SerializeField] private bool cancelByRightClick = true;

    private Vector3 currentTarget;
    private Tween jumpTween;

    /// <summary>
    ///     Jumps to target with specified key
    /// </summary>
    /// <param name="key">Key leading to the target jumping to</param>
    /// <param name="offset">Offset of the target towards the jumper at the end</param>
    /// <param name="onlyIfTargetOffScreen">Only jump if the target is offscreen</param>
    public void Jump(string key, Vector2? offset = null, bool onlyIfTargetOffScreen = false)
    {
        if (!targetList.ContainsKey(key)) throw new Exception($"Couldn't find target with key {key}");

        // find target
        (GameObject target, Renderer targetRenderer) = targetList[key];

        if (onlyIfTargetOffScreen && targetRenderer.isVisible) return;

        // get target position (preserve z value of camera)
        Vector2 targetPosition = target.transform.position;
        Vector2 targetOffset = offset ?? Vector2.zero;

        currentTarget = new(targetPosition.x - targetOffset.x, targetPosition.y - targetOffset.y, transform.position.z);

        if (Smooth)
        {
            jumpTween?.Kill();
            jumpTween = transform.DOMove(currentTarget, Time).SetEase(Ease.OutCubic);
        }
        else
        {
            // instantly set position if non-smooth
            Transform t = transform;
            t.position = new(currentTarget.x, currentTarget.y, t.position.z);
        }
    }

    private void Update()
    {
        if (cancelByRightClick && Input.GetMouseButtonDown(1) && jumpTween != null)
        {
            jumpTween.Kill();
            jumpTween = null;
        }
    }

    #region Target list manipulation

    public void AddTarget(string key, GameObject target)
    {
        if (target == null) throw new Exception("Game object tried to add to target list is null");

        if (target.TryGetComponent(out Renderer objRenderer))
        {
            AddTarget(key, target, objRenderer);
            return;
        }

        Debug.LogWarning(
            $"Couldn't find Renderer component on game object {target}, which was added to the target list"
        );
    }

    public void AddTarget(string key, GameObject target, Renderer targetRenderer)
    {
        if (targetList.ContainsKey(key))
        {
            targetList[key] = (target, targetRenderer);
            return;
        }

        targetList.Add(key, (target, targetRenderer));
    }

    public bool RemoveTarget(string key) => targetList.Remove(key);

    public GameObject GetTarget(string key) => HasKey(key) ? targetList[key].target : null;

    public bool HasKey(string key) => targetList.ContainsKey(key);

    #endregion
}