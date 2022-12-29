using UnityEngine;

public class KeyDoorField : MonoBehaviour
{
    public bool unlocked;
    public KeyManager.KeyColor color;
    private static readonly int unlockedString = Animator.StringToHash("Unlocked");

    public void Lock(bool locked)
    {
        unlocked = !locked;

        GetComponent<BoxCollider2D>().enabled = locked;

        Animator anim = GetComponent<Animator>();
        anim.SetBool(unlockedString, !locked);
    }
}