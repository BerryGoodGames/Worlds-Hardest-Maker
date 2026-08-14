using MyBox;
using UnityEngine;

public class KeySneezeParticles : MonoBehaviour
{
    [SerializeField] [MustBeAssigned] private ParticleSystem particles;
    
    public void Particles()
    {
        particles = particles != null ? particles : GetComponent<ParticleSystem>();
        
        particles.Play();
    }
}