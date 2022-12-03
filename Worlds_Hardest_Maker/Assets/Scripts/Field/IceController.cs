using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player"))
        {
            PlayerController controller = collider.GetComponent<PlayerController>();
            if (!controller.IsOnIce())
            {
                print("pawdo");
                // frame player entered ice
                // transition between normal ground and ice
                controller.rb.velocity = controller.movementInput * controller.GetCurrentSpeed();
            }
        }
    }
}
