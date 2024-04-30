using System;
using System.Collections.Generic;
using System.Reflection;
using Cinemachine.Utility;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

public class PlaceManager : MonoBehaviour
{
    public static PlaceManager Instance { get; private set; }

    [Separator("General sfx")] public SoundEffect DefaultPlaceSfx;
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
            FieldMode mode = (FieldMode)editMode;
            FieldManager.Instance.PlaceField(mode, rotation, playSound, matrixPosition);
            return;
        }

        AnchorController sheet = GetCurrentSheet();

        if (editMode ==
            // check field deletion
            EditModeManager.Delete)
        {
            bool deletedField = FieldManager.Instance.Remove(matrixPosition, true, sheet);

            // delete field
            if (deletedField && playSound) AudioManager.Instance.Play(GetSfx(editMode));

            // remove player if at deleted pos
            PlayerManager.Instance.RemoveAtPosIntersect(matrixPosition);

            return;
        }

        List<IManager> managers = new()
        {
            PlayerManager.Instance,
            BallManager.Instance,
            CoinManager.Instance,
            AnchorManager.Instance,
            KeyManager.Instance,
        };

        foreach (IManager manager in managers)
        {
            if (CheckManagerPlacement(editMode, playSound, manager, gridPosition)) break;
        }
    }

    private bool CheckManagerPlacement(EditMode editMode, bool playSound, IManager manager, Vector2 gridPosition)
    {
        if (!manager.CorrespondsToEditMode(editMode)) return false;

        ManagerParameters args = ManagerParameters.GetCurrentSheetParams(new() { Position = gridPosition, SurroundWithStartFields = true, });
        if (editMode.Attributes.IsKey) args.KeyColor = ((KeyMode)editMode).KeyColor;

        MethodInfo setMethod = manager.GetType().GetMethod(nameof(IManager<LevelObjectController>.SetInSheet));
        object result = setMethod.Invoke(manager, new object[] { args, });

        if (result is null || !playSound) return true;

        AudioManager.Instance.Play(GetSfx(editMode));

        if (editMode == EditModeManager.Anchor) AnchorManager.Instance.Select((AnchorController)result);

        return true;
    }

    public void PlacePath(EditMode editMode, Vector2 start, Vector2 end, int rotation = 0, bool playSound = false)
    {
        if (playSound) AudioManager.Instance.Play(GetSfx(editMode));

        LineForEach(start, end, pos => Place(editMode, pos, rotation));
    }

    [CanBeNull]
    public static AnchorController GetCurrentSheet() => AnchorAttachManager.Instance.InAttachMode ? AnchorManager.Instance.SelectedAnchor : null;

    public static void AttachToSheet(GameObject obj, [CanBeNull] AnchorController sheet, bool forceParent = true)
    {
        if (sheet == null) return;

        if (!obj.TryGetComponent(out AnchorAttachment attachment))
        {
            attachment = obj.AddComponent<AnchorAttachment>();
        }
        
        attachment.Anchor = sheet;

        if (forceParent && obj.transform.parent != sheet.AttachmentContainer) obj.transform.SetParent(sheet.AttachmentContainer);
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

        public PlaceSoundEffect(EditMode mode, string sound) : base(sound) => Mode = mode;

        public PlaceSoundEffect(EditMode mode, string sound, bool pitchRandomization, float pitchDeviation) : base(
            sound, pitchRandomization, pitchDeviation
        ) =>
            Mode = mode;
    }
}