using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Collections;
using System.Drawing;

public class MKey : MonoBehaviour
{
    public static MKey Instance { get; private set; }

    public enum KeyColor
    {
        GRAY, RED, GREEN, BLUE, YELLOW
    }

    public static readonly List<MGame.EditMode> KeyModes = new(new MGame.EditMode[]
    {
        MGame.EditMode.GRAY_KEY,
        MGame.EditMode.RED_KEY,
        MGame.EditMode.BLUE_KEY,
        MGame.EditMode.GREEN_KEY,
        MGame.EditMode.YELLOW_KEY
    });
    public static readonly List<MGame.EditMode> KeyDoorModes = new(new MGame.EditMode[]
    {
        MGame.EditMode.GRAY_KEY_DOOR_FIELD,
        MGame.EditMode.RED_KEY_DOOR_FIELD,
        MGame.EditMode.BLUE_KEY_DOOR_FIELD,
        MGame.EditMode.GREEN_KEY_DOOR_FIELD,
        MGame.EditMode.YELLOW_KEY_DOOR_FIELD
    });
    public static readonly List<MField.FieldType> KeyDoorTypes = new(new MField.FieldType[]
    {
        MField.FieldType.GRAY_KEY_DOOR_FIELD,
        MField.FieldType.RED_KEY_DOOR_FIELD,
        MField.FieldType.BLUE_KEY_DOOR_FIELD,
        MField.FieldType.GREEN_KEY_DOOR_FIELD,
        MField.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    public static List<MField.FieldType> CantPlaceFields = new(new MField.FieldType[]{
        MField.FieldType.WALL_FIELD,
        MField.FieldType.GRAY_KEY_DOOR_FIELD,
        MField.FieldType.RED_KEY_DOOR_FIELD,
        MField.FieldType.BLUE_KEY_DOOR_FIELD,
        MField.FieldType.GREEN_KEY_DOOR_FIELD,
        MField.FieldType.YELLOW_KEY_DOOR_FIELD
    });

    [PunRPC]
    public void SetKey(float mx, float my, KeyColor color)
    {
        if(CanPlace(mx, my))
        {
            Vector2 pos = new(mx, my);

            RemoveKey(mx, my);

            GameObject key = Instantiate(color.GetPrefabKey(), pos, Quaternion.identity, MGame.Instance.KeyContainer.transform);
                    
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", MGame.Instance.Playing);

            key.GetComponent<IntervalRandomAnimation>().enabled = MGame.Instance.KonamiActive;
        }
    }
    [PunRPC]
    public void RemoveKey(float mx, float my)
    {
        foreach(Transform key in MGame.Instance.KeyContainer.transform)
        {
            CKey controller = key.GetChild(0).GetComponent<CKey>();
            if (controller.keyPosition == new Vector2(mx, my))
            {
                DestroyImmediate(key.gameObject);

                if(MGame.Instance.Playing) controller.CheckAndUnlock(MPlayer.GetClientPlayer());
            }
        }
    }

    public static void SetKonamiMode(bool konami)
    {
        foreach(Transform key in MGame.Instance.KeyContainer.transform)
        {
            key.GetComponent<IntervalRandomAnimation>().enabled = konami;
        }
    }

    public static bool CanPlace(float mx, float my)
    {
        // conditions: no key there, covered by canplacefield or default, no player there
        return !MPlayer.IsPlayerThere(mx, my) && !IsKeyThere(mx, my) && !MField.IntersectingAnyFieldsAtPos(mx, my, CantPlaceFields.ToArray());
    }

    public static GameObject GetKey(float mx, float my)
    {
        GameObject container = MGame.Instance.KeyContainer;
        foreach (Transform key in container.transform)
        {
            if (key.GetChild(0).GetComponent<CKey>().keyPosition == new Vector2(mx, my))
            {
                return key.gameObject;
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
        return key != null && key.transform.GetChild(0).GetComponent<CKey>().color == color;
    }
    public static bool IsKeyThere(float mx, float my)
    {
        return GetKey(mx, my) != null;
    }

    public static bool IsKeyDoorEditMode(MGame.EditMode mode)
    {
        return KeyDoorModes.Contains(mode);
    }
    public static bool IsKeyEditMode(MGame.EditMode mode)
    {
        return KeyModes.Contains(mode);
    }

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
