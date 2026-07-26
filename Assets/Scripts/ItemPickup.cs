using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float holdDistance;
    [SerializeField] private LayerMask layer;

    [Header("Hold Feel")]
    [SerializeField] private float followStrength = 12f;   // higher = snappier/stiffer, lower = floatier/laggier
    [SerializeField] private float rotationStrength = 12f;
    [SerializeField] private float maxHoldSpeed = 20f;      // clamp so a big correction doesn't fling the item

    public GameObject currentPickup;
    private Rigidbody currentPickupRb;
    private bool isHolding;

    private CollisionDetectionMode originalCollisionMode;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * range, Color.red);

        if (Input.GetMouseButton(0) && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, layer) && !currentPickup)
        {
            Debug.Log(hit.collider.name);

            currentPickup = hit.collider.gameObject;
            currentPickupRb = currentPickup.GetComponent<Rigidbody>();

            originalCollisionMode = currentPickupRb.collisionDetectionMode;
            currentPickupRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            currentPickupRb.useGravity = false;
            currentPickupRb.linearDamping = 0f;
            currentPickupRb.angularDamping = 0f;

            isHolding = true;
        }
        else if (currentPickup && Input.GetMouseButtonUp(0))
        {
            currentPickupRb.useGravity = true;
            currentPickupRb.collisionDetectionMode = originalCollisionMode;
            // no velocity reset, whatever velocity it has right now is the throw

            isHolding = false;
            currentPickup = null;
            currentPickupRb = null;
        }
    }

    private void FixedUpdate()
    {
        if (isHolding && currentPickup)
        {
            MovePickupItem();
        }
    }

    private void MovePickupItem()
    {
        Vector3 targetPosition = transform.position + transform.forward * holdDistance;

        // Position: steer velocity toward the target point (spring-like)
        Vector3 toTarget = targetPosition - currentPickupRb.position;
        Vector3 desiredVelocity = toTarget * followStrength;
        currentPickupRb.linearVelocity = Vector3.ClampMagnitude(desiredVelocity, maxHoldSpeed);

        // Rotation: steer angular velocity toward matching the player's rotation
        Quaternion rotDelta = transform.rotation * Quaternion.Inverse(currentPickupRb.rotation);
        rotDelta.ToAngleAxis(out float angleDeg, out Vector3 axis);
        if (angleDeg > 180f) angleDeg -= 360f;

        if (Mathf.Abs(angleDeg) > Mathf.Epsilon)
        {
            currentPickupRb.angularVelocity = axis * (angleDeg * Mathf.Deg2Rad * rotationStrength);
        }
    }
}