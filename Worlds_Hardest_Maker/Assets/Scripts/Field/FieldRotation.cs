using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldRotation : MonoBehaviour
{
    // hippety hoppety
    public float duration;
    public Vector3 rotateAngle;
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

        StartCoroutine(Rotate(rotateAngle, duration));
    }

    private void OnMouseUpAsButton()
    {
        if (SelectionManager.Instance.Selecting || CopyManager.pasting || GameManager.Instance.Playing ||
            GameManager.Instance.CurrentEditMode !=
            GameManager.ConvertEnum<FieldType, EditMode>((FieldType)FieldManager.GetFieldType(gameObject))) return;

        if (GameManager.Instance.Multiplayer)
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