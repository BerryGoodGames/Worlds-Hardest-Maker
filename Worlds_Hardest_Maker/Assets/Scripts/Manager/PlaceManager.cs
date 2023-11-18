using System;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceManager : MonoBehaviour
{
    public static PlaceManager Instance { get; private set; }
    
    [SerializeField] private string defaultPlaceSfx = "Place";
    public PlaceSfx[] customPlaceSfx;

    /// <summary>
    /// Places edit mode at position
    /// </summary>
    /// <param name="editMode">the type of field/entity you want</param>
    /// <param name="position">position of the field/entity</param>
    /// <param name="rotation">rotation of the field/entity if possible</param>
    /// <param name="playSound">if it should play the place sound</param>
    public void Place(EditMode editMode, Vector2 position, int rotation = 0, bool playSound = false)
    {
        if(playSound)
            AudioManager.Instance.Play(GetSfx(editMode));
        
        Vector2 gridPosition = position.ConvertToGrid();
        Vector2Int matrixPosition = position.ConvertToMatrix();

        // check field placement
        if (editMode.IsFieldType())
        {
            FieldType type = editMode.ConvertTo<EditMode, FieldType>();
            if (!type.IsRotatable()) rotation = 0;
            FieldManager.Instance.SetField(matrixPosition, type, rotation);
        }
        // TODO: fix complexity by putting set methods in abstract class and make a general method to get the abstract classes
        else switch (editMode)
        {
            // check field deletion
            case EditMode.DeleteField:
                // delete field
                FieldManager.Instance.RemoveField(matrixPosition, true);

                // remove player if at deleted pos
                PlayerManager.Instance.RemovePlayerAtPosIntersect(matrixPosition);
                break;
            case EditMode.Player:
                PlayerManager.Instance.SetPlayer(gridPosition, true);
                break;
            case EditMode.AnchorBall:
                AnchorBallManager.SetAnchorBall(gridPosition);
                break;
            case EditMode.Coin:
                CoinManager.Instance.SetCoin(gridPosition);
                break;
            case EditMode.Anchor:
                // place new anchor + select
                AnchorController anchor = AnchorManager.Instance.SetAnchor(gridPosition);
                if (anchor != null) AnchorManager.Instance.SelectAnchor(anchor);
                break;
            default:
            {
                if (editMode.IsKey())
                {
                    // get key color
                    string editModeStr = editMode.ToString();
                    string keyColorStr = editModeStr.Remove(editModeStr.Length - 3);
                    KeyManager.KeyColor keyColor = keyColorStr.ToEnum<KeyManager.KeyColor>();

                    // place key
                    KeyManager.Instance.SetKey(gridPosition, keyColor);
                }
                break;
            }
        }
    }

    public void PlacePath(EditMode editMode, Vector2 start, Vector2 end, int rotation = 0, bool playSound = false)
    {
        if(playSound)
            AudioManager.Instance.Play(GetSfx(editMode));
        
        
        // generalized Bresenham's Line Algorithm optimized without /, find (unoptimized) algorithm here: https://www.uobabylon.edu.iq/eprints/publication_2_22893_6215.pdf
        // I tried my best to explain the variables, but I have no idea how it works

        Vector2 delta = (end - start).Abs();
        Vector2 increment = end - start;
        increment.Set(Mathf.Sign(increment.x), Mathf.Sign(increment.y));

        float cmpt = Mathf.Max(delta.x, delta.y); // max of both numbers
        float incrementD = -2 * Mathf.Abs(delta.x - delta.y); // increment of delta
        float incrementS = 2 * Mathf.Min(delta.x, delta.y); // I have no idea

        float error = incrementD + cmpt; // error of line
        Vector2 current = start;

        while (cmpt >= 0)
        {
            Place(editMode, current, rotation);
            cmpt -= 1;

            if (error >= 0 || delta.x > delta.y) current.x += increment.x;
            if (error >= 0 || delta.x <= delta.y) current.y += increment.y;
            if (error >= 0) error += incrementD;
            else error += incrementS;
        }
    }

    private string GetSfx(EditMode editMode)
    {
        string sfx = defaultPlaceSfx;

        foreach (PlaceSfx placeSfx in customPlaceSfx)
        {
            if (placeSfx.Mode == editMode)
            {
                sfx = placeSfx.Sound;
            }
        }

        return sfx;
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [Serializable]
    public struct PlaceSfx
    {   
        [SerializeField] public EditMode Mode;
        [SerializeField] public string Sound;

        public PlaceSfx(EditMode mode, string sound)
        {
            Mode = mode;
            Sound = sound;
        }
    }
    
}

