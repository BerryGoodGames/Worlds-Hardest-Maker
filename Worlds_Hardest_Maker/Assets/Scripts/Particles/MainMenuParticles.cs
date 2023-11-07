using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class MainMenuParticles : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        float camWidth = Utils.GetScreenDimensions(cam, null).x;

        ParticleSystem.ShapeModule shape = ps.shape;
        shape.scale = new(camWidth, camWidth, 1);
    }
}