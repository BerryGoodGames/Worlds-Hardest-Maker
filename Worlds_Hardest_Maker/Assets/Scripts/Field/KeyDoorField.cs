using UnityEngine;

public class KeyDoorField : MonoBehaviour
{
    public bool Unlocked;
    public KeyManager.KeyColor Color;
    private static readonly int unlockedString = Animator.StringToHash("Unlocked");

    public void Lock(bool locked)
    {
        Unlocked = !locked;

        GetComponent<BoxCollider2D>().enabled = locked;

        Animator anim = GetComponent<Animator>();
        anim.SetBool(unlockedString, !locked);
    }
}