using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Collections;
using System.Drawing;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public enum KeyColor
    {
        GRAY, RED, GREEN, BLUE, YELLOW
    }

    public static readonly List<GameManager.EditMode> KeyModes = new(new GameManager.EditMode[]
    {
        GameManager.EditMode.GRAY_KEY,
        GameManager.EditMode.RED_KEY,
        GameManager.EditMode.BLUE_KEY,
        GameManager.EditMode.GREEN_KEY,
        GameManager.EditMode.YELLOW_KEY
    });
    public static readonly List<GameManager.EditMode> KeyDoorModes = new(new GameManager.EditMode[]
    {
        GameManager.EditMode.GRAY_KEY_DOOR_FIELD,
        GameManager.EditMode.RED_KEY_DOOR_FIELD,
        GameManager.EditMode.BLUE_KEY_DOOR_FIELD,
        GameManager.EditMode.GREEN_KEY_DOOR_FIELD,
        GameManager.EditMode.YELLOW_KEY_DOOR_FIELD
    });
    public static readonly List<FieldManager.FieldType> KeyDoorTypes = new(new FieldManager.FieldType[]
    {
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static List<FieldManager.FieldType> CantPlaceFields = new(new FieldManager.FieldType[]{
        FieldManager.FieldType.WALL_FIELD,
        FieldManager.FieldType.GRAY_KEY_DOOR_FIELD,
        FieldManager.FieldType.RED_KEY_DOOR_FIELD,
        FieldManager.FieldType.BLUE_KEY_DOOR_FIELD,
        FieldManager.FieldType.GREEN_KEY_DOOR_FIELD,
        FieldManager.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    [PunRPC]
    public void SetKey(float mx, float my, KeyColor color)
    {
        if(CanPlace(mx, my))
        {
            Vector2 pos = new(mx, my);

            RemoveKey(mx, my);

            GameObject key = Instantiate(color.GetPrefabKey(), pos, Quaternion.identity, ReferenceManager.Instance.KeyContainer);
                    
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", GameManager.Instance.Playing);

            key.GetComponent<IntervalRandomAnimation>().enabled = GameManager.Instance.KonamiActive;
        }
    }

    public void SetKey(Vector2 pos, KeyColor color)
    {
        SetKey(pos.x, pos.y, color);
    }
    [PunRPC]
    public void RemoveKey(float mx, float my)
    {
        DestroyImmediate(GetKey(mx, my));
    }

    public static void SetKonamiMode(bool konami)
    {
        foreach(Transform key in ReferenceManager.Instance.KeyContainer)
        {
            key.GetComponent<IntervalRandomAnimation>().enabled = konami;
        }
    }

    public static bool CanPlace(float mx, float my)
    {
        // conditions: no key there, covered by canplacefield or default, no player there
        return !PlayerManager.IsPlayerThere(mx, my) && !IsKeyThere(mx, my) && !FieldManager.IntersectingAnyFieldsAtPos(mx, my, CantPlaceFields.ToArray());
    }

    public static GameObject GetKey(float mx, float my)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(new(mx, my), 0.01f, 128);
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<KeyController>() != null)
            {
                return hit.transform.parent.gameObject;
            }
        }
        return null;
    }
    public static GameObject GetKey(Vector2 pos)
    {
        return GetKey(pos.x, pos.y);
    }

    public static bool IsKeyThere(float mx, float my, KeyColor color)
    {
        GameObject key = GetKey(mx, my);
        return key != null && key.transform.GetChild(0).GetComponent<KeyController>().color == color;
    }
    public static bool IsKeyThere(float mx, float my)
    {
        return GetKey(mx, my) != null;
    }

    public static bool IsKeyDoorEditMode(GameManager.EditMode mode)
    {
        return KeyDoorModes.Contains(mode);
    }
    public static bool IsKeyEditMode(GameManager.EditMode mode)
    {
        return KeyModes.Contains(mode);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
