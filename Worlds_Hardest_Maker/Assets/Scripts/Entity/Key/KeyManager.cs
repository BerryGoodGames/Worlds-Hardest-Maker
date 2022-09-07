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

            GameObject key = Instantiate(color.GetPrefabKey(), pos, Quaternion.identity, GameManager.Instance.KeyContainer.transform);
                    
            Animator anim = key.GetComponent<Animator>();
            anim.SetBool("Playing", GameManager.Instance.Playing);

            key.GetComponent<IntervalRandomAnimation>().enabled = GameManager.Instance.KonamiActive;
        }
    }
    [PunRPC]
    public void RemoveKey(float mx, float my)
    {
        foreach(Transform key in GameManager.Instance.KeyContainer.transform)
        {
            KeyController controller = key.GetChild(0).GetComponent<KeyController>();
            if (controller.keyPosition == new Vector2(mx, my))
            {
                DestroyImmediate(key.gameObject);

                if(GameManager.Instance.Playing) controller.CheckAndUnlock(PlayerManager.GetClientPlayer());
            }
        }
    }

    public static void SetKonamiMode(bool konami)
    {
        foreach(Transform key in GameManager.Instance.KeyContainer.transform)
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
        GameObject container = GameManager.Instance.KeyContainer;
        foreach (Transform key in container.transform)
        {
            if (key.GetChild(0).GetComponent<KeyController>().keyPosition == new Vector2(mx, my))
            {
                return key.gameObject;
            }
        }
        return null;
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
