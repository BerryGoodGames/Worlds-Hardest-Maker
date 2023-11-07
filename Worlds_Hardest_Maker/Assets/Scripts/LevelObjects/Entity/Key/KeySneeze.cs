using UnityEngine;

public class KeySneeze : MonoBehaviour
{
    private ParticleSystem ps;
    private AudioSource audioSource;

    public void Particles()
    {
        ps = ps != null ? ps : GetComponent<ParticleSystem>();
        audioSource = audioSource != null ? audioSource : GetComponent<AudioSource>();

        ps.Play();
        audioSource.Play();
    }
}