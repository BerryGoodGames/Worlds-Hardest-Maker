using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GunController : MonoBehaviour
{
    // settings
    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private float reloadTime;
    [SerializeField] private int recoil;
    [SerializeField] private bool automatic;
    [SerializeField] private float cooldown;

    // normal variables
    private int ammo = 0;
    private float cooldownProgress;

    // references
    private Camera mainCamera;

    private void Awake()
    {
        // init variables
        ammo = maxAmmo;
        cooldownProgress = cooldown;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // progress cooldown
        cooldownProgress += Time.deltaTime;

        // shoot if clicked
        if (automatic && Input.GetMouseButton(0)) Shoot();
        else if (Input.GetMouseButtonDown(0)) Shoot();
    }

    private void Shoot()
    {
        // check if it can shoot
        if (ammo <= 0 || cooldownProgress <= cooldown) return;

        // calculate recoil
        Vector2 direction = (mainCamera.ScreenToWorldPoint(Input.mousePosition) - PlayerController.Main.transform.position).normalized;
        PlayerController.Main.Rigidbody.AddForce(direction * -recoil);

        // update status
        ammo--;
        cooldownProgress = 0;

        if(ammo <= 0) Reload();
    }

    private void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        // wait until it can reload
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
    }
}
