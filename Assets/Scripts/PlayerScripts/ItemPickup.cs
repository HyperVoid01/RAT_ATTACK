using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float radius;
    [SerializeField] private float holdDistance;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Camera camera;

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
        // Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, range, layer)
        // Physics.SphereCast(camera.transform.position, radius, camera.transform.forward, out RaycastHit hit, range, layer)
        
        if (Input.GetMouseButton(0) && Physics.SphereCast(camera.transform.position, radius, camera.transform.forward, out RaycastHit hit, range, layer) && !currentPickup)
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
        HUDManager.Instance.DisableInteractionText();
        
        //Vector3 targetPosition = transform.position + transform.forward * holdDistance;
        Transform camTransform = camera.transform;
        Vector3 targetPosition = camTransform.position + camTransform.forward * holdDistance;
        
        // Position: steer velocity toward the target point (spring-like)
        Vector3 toTarget = targetPosition - currentPickupRb.position;
        Vector3 desiredVelocity = toTarget * followStrength;
        currentPickupRb.linearVelocity = Vector3.ClampMagnitude(desiredVelocity, maxHoldSpeed);

        // Rotation: steer angular velocity toward matching the player's rotation
        // Quaternion rotDelta = transform.rotation * Quaternion.Inverse(currentPickupRb.rotation);
        Quaternion rotDelta = camTransform.rotation * Quaternion.Inverse(currentPickupRb.rotation);
        rotDelta.ToAngleAxis(out float angleDeg, out Vector3 axis);
        if (angleDeg > 180f) angleDeg -= 360f;

        if (Mathf.Abs(angleDeg) > Mathf.Epsilon)
        {
            currentPickupRb.angularVelocity = axis * (angleDeg * Mathf.Deg2Rad * rotationStrength);
        }
    }

    private void OnDrawGizmos()
    {
        if (camera == null) return;

        Transform camTransform = camera.transform;
        Vector3 start = camTransform.position;
        Vector3 direction = camTransform.forward;

        bool didHit = Physics.SphereCast(start, radius, direction, out RaycastHit hit, range, layer);
        float distance = didHit ? hit.distance : range;
        Vector3 end = start + direction * distance;

        Gizmos.color = didHit ? Color.green : Color.red;

        // Start and end spheres of the sweep
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);

        // Side lines to suggest the swept "capsule" silhouette
        Vector3 up = camTransform.up * radius;
        Vector3 right = camTransform.right * radius;
        Gizmos.DrawLine(start + up, end + up);
        Gizmos.DrawLine(start - up, end - up);
        Gizmos.DrawLine(start + right, end + right);
        Gizmos.DrawLine(start - right, end - right);

        if (didHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.1f);
        }
    }
}