using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class ShotgunController : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private ParticleSystem fireParticles;

    private ParticleSystem bulletParticle;

    private float currentAngle;
    
    private void Update()
    {
        LookAtMouse();

        if (Input.GetMouseButtonDown(0)) Fire();

        return;
        
        void LookAtMouse()
        {
            Vector2 position = transform.position;
            Vector2 mousePosition = MouseManager.Instance.MouseWorldPos;
            
            currentAngle = LookAt(position, mousePosition);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    private void Fire()
    {
        fireParticles.Play();
        
        // rotate bullet
        ParticleSystem.MainModule main = bulletParticle.main;
        main.startRotationZ = currentAngle / 180 * Mathf.PI;
        
        // load colliders for bullet to check
        for (int i = 0; i < AnchorBallManager.Instance.AnchorBallList.Count; i++)
        {
            AnchorBallController anchorBall = AnchorBallManager.Instance.AnchorBallList[i];
            bulletParticle.trigger.SetCollider(i, anchorBall.GetComponent<CircleCollider2D>());
        }

        bulletParticle.Play();
    }

    private void OnParticleTrigger()
    {
        // particles
        List<ParticleSystem.Particle> enter = new();

        // get
        int numEnter = bulletParticle.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        
        // iterate
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            
            // get anchor ball colliding with bullet
            Collider2D[] hits = Physics2D.OverlapCircleAll(p.position, 0.1f, LayerManager.Instance.Layers.Entity);
            foreach (Collider2D hit in hits)
            {
                if (!hit.CompareTag("AnchorBallObject")) continue;
                
                // launch ball
                hit.GetComponent<Rigidbody2D>().AddForce(p.velocity, ForceMode2D.Impulse);

                break;
            }
            
            enter[i] = p;
        }
        
        bulletParticle.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }

    private void OnParticleCollision(GameObject other)
    {
        print(other.name);
    }

    private static float LookAt(Vector2 here, Vector2 there) => Vector2.SignedAngle(Vector2.right, there - here);

    private void Awake() => bulletParticle = GetComponent<ParticleSystem>();
}
