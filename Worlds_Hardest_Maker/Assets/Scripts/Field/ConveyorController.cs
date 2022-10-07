using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    [SerializeField] private float strength;
    public float Strength { get { return strength; } set { strength = value; } }
    public float Rotation { get { return transform.rotation.z; } }

    public void Rotate()
    {
        rotationController.StartRotation();
    }

    private void Start()
    {
        rotationController = GetComponent<FieldRotation>();
    }
}
