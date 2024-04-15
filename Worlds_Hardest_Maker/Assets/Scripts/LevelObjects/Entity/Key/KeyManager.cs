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


    public KeyController SetKeyInSheet(Vector2 position, KeyColor color, [CanBeNull] AnchorController sheet)
    {
        Transform container = AnchorAttachManager.Instance.InAttachMode
            ? AnchorAttachManager.GetCurrentAnchorContainer()
            : ReferenceManager.Instance.KeyContainer;
        
        if (!CanPlace(position, sheet)) return null;

        // remove other key (which has mby other color)
        RemoveKeyInSheet(position, sheet);

        KeyController key = Instantiate(
            color.GetPrefabKey(), 
            position, Quaternion.identity,
            container
        );

        key.Color = color;

        // setup idle animation
        key.Animator.SetBool(playing, LevelSessionEditManager.Instance.Playing);

        // setup konami code animation
        key.KonamiAnimation.enabled = KonamiManager.Instance.KonamiActive;
        
        PlaceManager.AttachToSheet(key.gameObject, sheet);

        return key;
    }

    public KeyController SetKey(Vector2 position, KeyColor color) => SetKeyInSheet(position, color, PlaceManager.GetCurrentSheet());

    public void RemoveKey(Vector2 position)
    {
        KeyController key = GetKey(position);

        if (key == null) return;

        // un-cache
        Keys.Remove(key);

        // destroy
        DestroyImmediate(key.transform.gameObject);
    }
    
    public void RemoveKeyInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        KeyController key = GetKeyInSheet(position, sheet);

        if (key == null) return;

        // un-cache
        Keys.Remove(key);

        // destroy
        DestroyImmediate(key.transform.gameObject);
    }

    public static bool CanPlace(Vector2 position) => CanPlace(position, PlaceManager.GetCurrentSheet());
    
    public static bool CanPlace(Vector2 position, [CanBeNull] AnchorController sheet) =>
        // conditions: no key there, covered by canplacefield or default, no player there
        !PlayerManager.IsPlayerThere(position)
        && !IsKeyThereInSheet(position, sheet)
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

    public static KeyController GetKeyInSheet(Vector2 position, [CanBeNull] AnchorController sheet)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Key")) continue;
            if (!hit.TryGetComponent(out KeyController key)) continue;
            if (IsKeyInSheet(key, sheet)) return key;
        }

        return null;
    }

    public static bool IsKeyInSheet(KeyController coin, [CanBeNull] AnchorController sheet)
    {
        bool hasAttachment = coin.TryGetComponent(out AnchorAttachment attachment);
        
        // shorthand to:
        bool globalSheet = sheet == null;
        if (hasAttachment && globalSheet) return false;
        if (hasAttachment && attachment.Anchor != sheet) return false;
        if (!hasAttachment && !globalSheet) return false;
        return true;

        // return (globalSheet && !hasAttachment) || (hasAttachment && !globalSheet && attachment.Anchor == sheet);
    }
    
    public static bool IsKeyThere(Vector2 position, KeyColor color)
    {
        KeyController key = GetKey(position);
        return key != null && key.Color == color;
    }

    public static bool IsKeyThere(Vector2 position) => GetKey(position) != null;

    public static bool IsKeyThereInSheet(Vector2 position, KeyColor color, [CanBeNull] AnchorController sheet)
    {
        KeyController key = GetKeyInSheet(position, sheet);
        return key != null && key.Color == color;
    }

    public static bool IsKeyThereInSheet(Vector2 position, [CanBeNull] AnchorController sheet) => GetKeyInSheet(position, sheet) != null;
    
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