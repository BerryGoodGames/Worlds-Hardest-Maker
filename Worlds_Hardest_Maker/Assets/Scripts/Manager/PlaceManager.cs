using System;
using Cinemachine.Utility;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceManager : MonoBehaviour
{
    public static PlaceManager Instance { get; private set; }

    [Separator("General sfx")]
    public SoundEffect DefaultPlaceSfx;
    [SerializeField] private PlaceSoundEffect[] customPlaceSfx;
    
    [Separator("Konami sfx")] [SerializeField] private SoundEffect konamiPlaceSfx;
    [SerializeField] private PlaceSoundEffect[] customKonamiPlaceSfx;

    /// <summary>
    ///     Places edit mode at position
    /// </summary>
    /// <param name="editMode">the type of field/entity you want</param>
    /// <param name="position">position of the field/entity</param>
    /// <param name="rotation">rotation of the field/entity if possible</param>
    /// <param name="playSound">if it should play the place sound</param>
    public void Place(EditMode editMode, Vector2 position, int rotation = 0, bool playSound = false)
    {
        if (AnchorBlockManager.Instance.DraggingBlock) return;

        Vector2 gridPosition = position.ConvertToGrid();
        Vector2Int matrixPosition = position.ConvertToMatrix();

        // check field placement
        if (editMode.Attributes.IsField)
        {
            FieldMode type = (FieldMode)editMode;
            FieldManager.Instance.PlaceField(type, rotation, playSound, matrixPosition);
            return;
        }

        // TODO: fix complexity by putting set methods in abstract class and make a general method to get the abstract classes
        if (editMode ==
            // check field deletion
            EditModeManager.Delete)
        {
            // delete field
            if (FieldManager.Instance.RemoveField(matrixPosition, true) && playSound) AudioManager.Instance.Play(GetSfx(editMode));

            // remove player if at deleted pos
            PlayerManager.Instance.RemovePlayerAtPosIntersect(matrixPosition);
        }
        else if (editMode == EditModeManager.Player)
        {
            if (PlayerManager.Instance.SetPlayer(gridPosition, true) is not null && playSound) AudioManager.Instance.Play(GetSfx(editMode));
        }
        else if (editMode == EditModeManager.AnchorBall)
        {
            if (AnchorBallManager.SetAnchorBall(gridPosition) is not null && playSound) AudioManager.Instance.Play(GetSfx(editMode));
        }
        else if (editMode == EditModeManager.Coin)
        {
            if (CoinManager.SetCoin(gridPosition) is not null && playSound) AudioManager.Instance.Play(GetSfx(editMode));
        }
        else if (editMode == EditModeManager.Anchor)
        {
            // place new anchor + select
            AnchorController anchor = AnchorManager.Instance.SetAnchor(gridPosition);
            if (anchor is not null && playSound)
            {
                AudioManager.Instance.Play(GetSfx(editMode));
                AnchorManager.Instance.SelectAnchor(anchor);
            }
        }
        else if (editMode.Attributes.IsKey)
        {
            // get key color
            KeyColor keyColor = ((KeyMode)editMode).KeyColor;

            // place key
            if (KeyManager.Instance.SetKey(gridPosition, keyColor) is not null && playSound) AudioManager.Instance.Play(GetSfx(editMode));
        }
    }

    public void PlacePath(EditMode editMode, Vector2 start, Vector2 end, int rotation = 0, bool playSound = false)
    {
        if (playSound) AudioManager.Instance.Play(GetSfx(editMode));

        LineForEach(start, end, pos => Place(editMode, pos, rotation));
    }

    public static void RemoveEntitiesAt(Vector2 position, LayerMask entityLayer)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(position, entityLayer);

        foreach (Collider2D hit in hits) hit.GetComponent<EntityController>().Delete();
    }

    private static void LineForEach(Vector2 start, Vector2 end, Action<Vector2> action)
    {
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
            action.Invoke(current);

            cmpt -= 1;

            if (error >= 0 || delta.x > delta.y) current.x += increment.x;
            if (error >= 0 || delta.x <= delta.y) current.y += increment.y;
            if (error >= 0) error += incrementD;
            else error += incrementS;
        }
    }

    public SoundEffect GetSfx(EditMode editMode)
    {
        SoundEffect sfx = KonamiManager.Instance.KonamiActive ? konamiPlaceSfx : DefaultPlaceSfx;
        
        PlaceSoundEffect[] soundCollection = KonamiManager.Instance.KonamiActive ? customKonamiPlaceSfx : customPlaceSfx;

        foreach (PlaceSoundEffect placeSfx in soundCollection)
        {
            if (placeSfx.Mode != editMode) continue;

            sfx = placeSfx;
            break;
        }

        return sfx;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [Serializable]
    public class PlaceSoundEffect : SoundEffect
    {
        [SerializeField] public EditMode Mode;
        
        public PlaceSoundEffect(EditMode mode, string sound) : base(sound)
        {
            Mode = mode;
        }

        public PlaceSoundEffect(EditMode mode, string sound, bool pitchRandomization, float pitchDeviation) : base(
            sound, pitchRandomization, pitchDeviation
        )
        {
            Mode = mode;
        }
    }
}