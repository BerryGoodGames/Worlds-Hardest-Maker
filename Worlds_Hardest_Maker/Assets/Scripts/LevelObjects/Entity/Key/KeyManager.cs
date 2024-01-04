using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using Photon.Pun;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    [UsedImplicitly] public static readonly List<FieldMode> CannotPlaceFields = new();

    private static readonly int playing = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    [ReadOnly] public List<KeyController> Keys = new();

    [PunRPC]
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
        key.Animator.SetBool(playing, EditModeManagerOther.Instance.Playing);

        // setup konami code animation
        key.KonamiAnimation.enabled = KonamiManager.Instance.KonamiActive;

        return key;
    }

    [PunRPC]
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

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }

    public void ActivateAnimations()
    {
        // activate key animations
        foreach (KeyController key in Keys)
        {
            // TODO: refactor that you don't have to access the animator and just wrap it into a property (also do this for coins etc.)
            // REMEMBER: you shouldn't have to know how the internals work to use it!
            key.Animator.SetBool(playing, true);
            key.Animator.SetBool(pickedUp, key.PickedUp);
        }
    }

    public void ResetStates()
    {
        foreach (KeyController key in Keys)
        {
            key.PickedUp = false;

            key.Animator.SetBool(playing, false);
            key.Animator.SetBool(pickedUp, false);
        }
    }
}