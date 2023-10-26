using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class ShotgunController : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private ParticleSystem fireParticles;

    [SerializeField] [InitializationField] [MustBeAssigned]
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

        ParticleSystem.MainModule main = bulletParticle.main;
        main.startRotationZ = -currentAngle / 180 * Mathf.PI;
        
        bulletParticle.Play();
    }

    private static float LookAt(Vector2 here, Vector2 there) => Vector2.SignedAngle(Vector2.right, there - here);
}
