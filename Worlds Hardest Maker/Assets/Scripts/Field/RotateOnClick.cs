using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateOnClick : MonoBehaviour
{
    // hippety hoppety
    public float duration;
    public Vector3 rotateAngle;
    private bool rotating;

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
        GetComponent<BoxCollider2D>().isTrigger = false;
    }

    public void StartRotation()
    {
        if (!rotating && !EventSystem.current.IsPointerOverGameObject())
        {
            GetComponent<BoxCollider2D>().isTrigger = true;

            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Rotate");

            StartCoroutine(Rotate(rotateAngle, duration));
        }
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.Filling && !GameManager.Instance.Playing && GameManager.Instance.CurrentEditMode.ToString() == FieldManager.GetFieldType(gameObject).ToString()) StartRotation();
    }
}
