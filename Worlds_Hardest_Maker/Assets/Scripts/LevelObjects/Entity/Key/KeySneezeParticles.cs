using NaughtyAttributes;
using UnityEngine;

public class KeySneezeParticles : MonoBehaviour
{
    [SerializeField] [Required] private ParticleSystem particles;
    
    public void Particles()
    {
        particles = particles != null ? particles : GetComponent<ParticleSystem>();
        
        particles.Play();
    }
}