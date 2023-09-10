using System.Collections.Generic;
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

    public static readonly List<EditMode> KeyModes = new(new[]
    {
        EditMode.GrayKey,
        EditMode.RedKey,
        EditMode.BlueKey,
        EditMode.GreenKey,
        EditMode.YellowKey
    });

    public static readonly List<EditMode> KeyDoorModes = new(new[]
    {
        EditMode.GrayKeyDoorField,
        EditMode.RedKeyDoorField,
        EditMode.BlueKeyDoorField,
        EditMode.GreenKeyDoorField,
        EditMode.YellowKeyDoorField
    });

    public static readonly List<FieldType> KeyDoorTypes = new(new[]
    {
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField
    });

    public static List<FieldType> CannotPlaceFields = new(new[]
    {
        FieldType.WallField,
        FieldType.GrayKeyDoorField,
        FieldType.RedKeyDoorField,
        FieldType.BlueKeyDoorField,
        FieldType.GreenKeyDoorField,
        FieldType.YellowKeyDoorField
    });

    private static readonly int playingString = Animator.StringToHash("Playing");

    [PunRPC]
    public void SetKey(float mx, float my, KeyColor color)
    {
        if (!CanPlace(mx, my)) return;

        Vector2 pos = new(mx, my);

        RemoveKey(mx, my);

        GameObject key = Instantiate(color.GetPrefabKey(), pos, Quaternion.identity,
            ReferenceManager.Instance.KeyContainer);

        Animator anim = key.GetComponent<Animator>();
        anim.SetBool(playingString, EditModeManager.Instance.Playing);

        key.GetComponent<IntervalRandomAnimation>().enabled = KonamiManager.KonamiActive;
    }

    public void SetKey(Vector2 pos, KeyColor color) => SetKey(pos.x, pos.y, color);

    [PunRPC]
    public void RemoveKey(float mx, float my) => DestroyImmediate(GetKey(mx, my));

    public static void SetKonamiMode(bool konami)
    {
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            key.GetComponent<IntervalRandomAnimation>().enabled = konami;
        }
    }

    public static bool CanPlace(float mx, float my) =>
        // conditions: no key there, covered by canplacefield or default, no player there
        !PlayerManager.IsPlayerThere(mx, my) &&
        !IsKeyThere(mx, my) &&
        !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CannotPlaceFields.ToArray());

    public static GameObject GetKey(float mx, float my)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<KeyController>() != null) return hit.transform.parent.gameObject;
        }

        return null;
    }

    public static GameObject GetKey(Vector2 pos) => GetKey(pos.x, pos.y);

    public static bool IsKeyThere(float mx, float my, KeyColor color)
    {
        GameObject key = GetKey(mx, my);
        return key != null && key.transform.GetChild(0).GetComponent<KeyController>().Color == color;
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