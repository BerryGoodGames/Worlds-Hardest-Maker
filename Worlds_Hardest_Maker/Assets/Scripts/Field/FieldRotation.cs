using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        if(!TryGetComponent(out boxCollider))
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
        if(disableCollision)
            boxCollider.isTrigger = false;
    }

    [PunRPC]
    public void StartRotation()
    {
        if (!rotating && !EventSystem.current.IsPointerOverGameObject())
        {
            if(disableCollision)
                boxCollider.isTrigger = true;

            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Rotate");

            StartCoroutine(Rotate(rotateAngle, duration));
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!GameManager.Instance.Filling && !GameManager.Instance.Playing && GameManager.Instance.CurrentEditMode == GameManager.ConvertEnum<FieldManager.FieldType, GameManager.EditMode>((FieldManager.FieldType)FieldManager.GetFieldType(gameObject)))
        {
            if (GameManager.Instance.Multiplayer)
            {
                PhotonView view = PhotonView.Get(this);
                view.RPC("StartRotation", RpcTarget.All);
            } else
            {
                StartRotation();
            }
        }
    }
}