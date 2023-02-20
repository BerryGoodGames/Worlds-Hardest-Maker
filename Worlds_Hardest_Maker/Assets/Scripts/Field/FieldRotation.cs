using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class FieldRotation : MonoBehaviour
{
    // hippety hoppety
    [FormerlySerializedAs("duration")] public float Duration;
    [FormerlySerializedAs("rotateAngle")] public Vector3 RotateAngle;
    private bool rotating;
    [SerializeField] private bool disableCollision;
    private BoxCollider2D boxCollider;
    private static readonly int rotateString = Animator.StringToHash("Rotate");

    private void Start()
    {
        if (!TryGetComponent(out boxCollider))
            disableCollision = false;
    }

    private IEnumerator Rotate(Vector3 angles, float d)
    {
        rotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;

        for (float t = 0; t < d; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / d);
            yield return null;
        }

        transform.rotation = endRotation;
        rotating = false;

        if (disableCollision)
            boxCollider.isTrigger = false;
    }

    [PunRPC]
    public void StartRotation()
    {
        if (rotating || EventSystem.current.IsPointerOverGameObject()) return;

        if (disableCollision)
            boxCollider.isTrigger = true;

        Animator anim = GetComponent<Animator>();
        anim.SetTrigger(rotateString);

        StartCoroutine(Rotate(RotateAngle, Duration));
    }

    private void OnMouseUpAsButton()
    {
        if (SelectionManager.Instance.Selecting || CopyManager.Pasting || EditModeManager.Instance.Playing ||
            EditModeManager.Instance.CurrentEditMode !=
            EnumUtils.ConvertEnum<FieldType, EditMode>((FieldType)FieldManager.GetFieldType(gameObject))) return;

        if (MultiplayerManager.Instance.Multiplayer)
        {
            PhotonView view = PhotonView.Get(this);
            view.RPC("StartRotation", RpcTarget.All);
        }
        else
        {
            StartRotation();
        }
    }
}