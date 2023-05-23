using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Main { get; private set; }

    // variables set in inspector
    [SerializeField] private float speed;

    // references
    private CharacterController2D characterController;
    [HideInInspector] public Rigidbody2D Rigidbody;

    private void Awake()
    {
        // init variables
        characterController = GetComponent<CharacterController2D>();
        Rigidbody = GetComponent<Rigidbody2D>();

        Main ??= this;
    }

    private void FixedUpdate()
    {
        // move player by input
        characterController.Move(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, false, false);
    }
}
