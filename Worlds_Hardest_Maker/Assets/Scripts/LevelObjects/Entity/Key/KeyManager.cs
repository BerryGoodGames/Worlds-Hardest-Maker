using System.Collections.Generic;
using MyBox;
using Photon.Pun;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public enum KeyColor
    {
        Gray,
        Red,
        Green,
        Blue,
        Yellow,
    }

    public static readonly List<EditMode> KeyModes = new()
    {
        EditMode.GrayKey,
        EditMode.RedKey,
        EditMode.BlueKey,
        EditMode.GreenKey,
        EditMode.YellowKey,
    };

    public static readonly List<EditMode> KeyDoorModes = new()
    {
        EditMode.GrayKeyDoorField,
        EditMode.RedKeyDoorField,
        EditMode.BlueKeyDoorField,
        EditMode.GreenKeyDoorField,
        EditMode.YellowKeyDoorField,
    };

    public static readonly List<FieldType> KeyDoorTypes = new()
    {
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField,
    };

    public static readonly List<FieldType> CannotPlaceFields = new()
    {
        FieldType.WallField,
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField,
    };

    private static readonly int playing = Animator.StringToHash("Playing");
    private static readonly int pickedUp = Animator.StringToHash("PickedUp");

    [ReadOnly] public List<KeyController> Keys = new();

    [PunRPC]
    public void SetKey(Vector2 position, KeyColor color)
    {
        if (!CanPlace(position)) return;

        // remove other key (which has mby other color)
        RemoveKey(position);

        KeyController key = Instantiate(
            color.GetPrefabKey(), position, Quaternion.identity,
            ReferenceManager.Instance.KeyContainer
        );

        key.Color = color;

        // setup idle animation
        key.Animator.SetBool(playing, EditModeManager.Instance.Playing);

        // setup konami code animation
        key.KonamiAnimation.enabled = KonamiManager.Instance.KonamiActive;
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
        !PlayerManager.IsPlayerThere(position) &&
        !IsKeyThere(position) &&
        !FieldManager.IntersectingAnyFieldsAtPos(position, CannotPlaceFields.ToArray());

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

    public static bool IsKeyDoorEditMode(EditMode mode) => KeyDoorModes.Contains(mode);

    public static bool IsKeyEditMode(EditMode mode) => KeyModes.Contains(mode);

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