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
        Yellow
    }

    public static readonly List<EditMode> KeyModes = new()
    {
        EditMode.GrayKey,
        EditMode.RedKey,
        EditMode.BlueKey,
        EditMode.GreenKey,
        EditMode.YellowKey
    };

    public static readonly List<EditMode> KeyDoorModes = new()
    {
        EditMode.GrayKeyDoorField,
        EditMode.RedKeyDoorField,
        EditMode.BlueKeyDoorField,
        EditMode.GreenKeyDoorField,
        EditMode.YellowKeyDoorField
    };

    public static readonly List<FieldType> KeyDoorTypes = new()
    {
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField
    };

    public static readonly List<FieldType> CannotPlaceFields = new()
    {
        FieldType.WallField,
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField
    };

    private static readonly int playingString = Animator.StringToHash("Playing");

    [ReadOnly] public List<KeyController> Keys;

    [PunRPC]
    public void SetKey(float mx, float my, KeyColor color)
    {
        if (!CanPlace(mx, my)) return;

        Vector2 pos = new(mx, my);

        // remove other key (which has mby other color)
        RemoveKey(mx, my);

        GameObject keyObject = Instantiate(color.GetPrefabKey(), pos, Quaternion.identity,
            ReferenceManager.Instance.KeyContainer);

        KeyController key = keyObject.transform.GetChild(0).GetComponent<KeyController>();
        key.Color = color;

        // setup idle animation
        key.Animator.SetBool(playingString, EditModeManager.Instance.Playing);

        // setup konami code animation
        key.KonamiAnimation.enabled = KonamiManager.KonamiActive;
    }

    public void SetKey(Vector2 pos, KeyColor color) => SetKey(pos.x, pos.y, color);

    [PunRPC]
    public void RemoveKey(float mx, float my)
    {
        KeyController key = GetKey(mx, my);

        if (key == null) return;

        // un-cache
        Keys.Remove(key);

        // destroy
        DestroyImmediate(key.transform.gameObject);
    }

    public static void SetKonamiMode(bool konami)
    {
        foreach (KeyController key in Instance.Keys)
        {
            key.KonamiAnimation.enabled = konami;
        }
    }

    public static bool CanPlace(float mx, float my) =>
        // conditions: no key there, covered by canplacefield or default, no player there
        !PlayerManager.IsPlayerThere(mx, my) &&
        !IsKeyThere(mx, my) &&
        !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CannotPlaceFields.ToArray());

    public static KeyController GetKey(float mx, float my)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, LayerManager.Instance.Layers.Entity);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out KeyController controller)) return controller;
        }

        return null;
    }

    public static KeyController GetKey(Vector2 pos) => GetKey(pos.x, pos.y);

    public static bool IsKeyThere(float mx, float my, KeyColor color)
    {
        KeyController key = GetKey(mx, my);
        return key != null && key.Color == color;
    }

    public static bool IsKeyThere(float mx, float my) => GetKey(mx, my) != null;

    public static bool IsKeyDoorEditMode(EditMode mode) => KeyDoorModes.Contains(mode);

    public static bool IsKeyEditMode(EditMode mode) => KeyModes.Contains(mode);

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}