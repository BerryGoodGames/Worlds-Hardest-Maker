using MyBox;
using UnityEngine;

public class AnchorBlockDropdown : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private PlaySoundEffect soundEffect;

    private bool hasInitialized;
    
    public void PlaySoundEffect()
    {
        if(hasInitialized) soundEffect.Play();
    }

    private void Start()
    {
        hasInitialized = true;
    }
}
