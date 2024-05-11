using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class AnchorBlockDropdown : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private PlaySoundEffect soundEffect;
    
    private bool hasInitialized;
    
    public void PlaySoundEffect()
    {
        if (hasInitialized) soundEffect.Play();
    }
    
    private void Start() => hasInitialized = true;
}