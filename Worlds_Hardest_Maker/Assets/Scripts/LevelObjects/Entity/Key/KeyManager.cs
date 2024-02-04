using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();

    private static readonly int playing = Animator.StringToHash("Playing");

    [ReadOnly] public List<KeyController> Keys = new();
    [ReadOnly] public List<KeyController> CollectedKeys = new();


    public KeyController SetKey(Vector2 position, KeyColor color)
    {
        if (!CanPlace(position)) return null;

        // remove other key (which has mby other color)
        RemoveKey(position);

        KeyController key = Instantiate(
            color.GetPrefabKey(), position, Quaternion.identity,
            ReferenceManager.Instance.KeyContainer
        );

        key.Color = color;

        // setup idle animation
        key.Animator.SetBool(playing, LevelSessionEditManager.Instance.Playing);

        // setup konami code animation
        key.KonamiAnimation.enabled = KonamiManager.Instance.KonamiActive;

        return key;
    }


    public void RemoveKey(Vector2 position)
    {
        KeyController key = GetKey(position);

        if (key == null) return;

        // un-cache
        Keys.Remove(key);

        // destroy
        DestroyImmediate(key.transform.gameObject);
    }

    public static bool CanPlace(Vector2 position) =>
        // conditions: no key there, covered by canplacefield or default, no player there
        !PlayerManager.IsPlayerThere(position)
        && !IsKeyThere(position)
        && !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray());

    public static KeyController GetKey(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out KeyController controller)) return controller;
        }

        return null;
    }

    public static bool IsKeyThere(Vector2 position, KeyColor color)
    {
        KeyController key = GetKey(position);
        return key != null && key.Color == color;
    }

    public static bool IsKeyThere(Vector2 position) => GetKey(position) != null;

    public bool AllKeysCollected(KeyColor color)
    {
        // check if every key of specific color is picked up
        foreach (KeyController key in Keys)
        {
            if (!key.Collected && key.Color == color) return false;
        }

        return true;
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public void ActivateAnimations() => Keys.ForEach(key => key.ActivateAnimation());
}