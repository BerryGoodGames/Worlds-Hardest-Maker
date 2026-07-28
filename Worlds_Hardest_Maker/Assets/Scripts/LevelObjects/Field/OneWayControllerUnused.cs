using UnityEngine;

[DefaultExecutionOrder(-1)]
public class OneWayController : MonoBehaviour
{

    [SerializeField] private float offset;
    
    private BoxCollider2D collider;
    private Transform playerTransform;
    private Rigidbody2D playerCollider;
    
    void Start()
    {
        collider = GetComponentInChildren<BoxCollider2D>();
        playerTransform = PlayerManager.Instance.Player.transform;
        playerCollider = playerTransform.GetComponent<Rigidbody2D>();
    }

    
    // Only works properly if rotated by 90 degrees and player is square
    void FixedUpdate()
    {
        Vector2 playerDir = playerTransform.position - (transform.position + transform.up * ((collider.size.x + playerTransform.lossyScale.x) / 2 + offset));
        float dotProduct = Vector2.Dot(transform.up, playerDir);
        collider.enabled = dotProduct > 0;
        print(dotProduct);
    }
}
