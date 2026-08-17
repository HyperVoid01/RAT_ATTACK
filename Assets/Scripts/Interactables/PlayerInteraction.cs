using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach;
    public Interactable currentInteractable;

    [Header("Held Items")]
    [Tooltip("Reference to the pickup script so we know what's currently held.")]
    [SerializeField] private ItemPickup itemPickup;
    [Tooltip("Kept separate from ItemPickup's Mouse0 (used to carry/hold items).")]
    public KeyCode itemHoldKey = KeyCode.Mouse1;
    public KeyCode interactKey = KeyCode.E;

    // Bindings own their own start/stop/force-stop bookkeeping - add more here
    // for future hold-based features without touching Update().
    private HoldInputBinding lookHold;
    private HoldInputBinding itemHold;
    
    public bool isActive = true;

    private void Awake()
    {
        lookHold = new HoldInputBinding(interactKey, () => currentInteractable as IHoldInteraction);
        itemHold = new HoldInputBinding(itemHoldKey, ResolveHeldHoldTarget);
    }

    private IHoldInteraction ResolveHeldHoldTarget()
    {
        if (itemPickup && itemPickup.currentPickup)
        {
            return itemPickup.currentPickup.GetComponent<IHoldInteraction>();
        }

        return null;
    }

    private void Update()
    {
        if (!isActive)
            return;
        
        CheckInteraction();

        if (Input.GetKeyDown(interactKey) && currentInteractable)
        {
            currentInteractable.Interact();
        }

        lookHold.Tick();
        itemHold.Tick();
    }

    private void OnDisable()
    {
        lookHold?.ForceStop();
        itemHold?.ForceStop();
    }

    private void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // If collides with anything within player reach
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Pizza")) // If looking at an interactable object
            {
                if (!hit.collider.GetComponent<Interactable>())
                    return;

                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                // If there is a currentInteractable, and it is not the newInteractable
                if (currentInteractable && newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }

                if (newInteractable.enabled)
                {
                    SetNewCurrentInteractable(newInteractable);
                }
                else // If new interactable is not enabled
                {
                    DisableCurrentInteractable();
                }

            }
            else // If not an interactable
            {
                DisableCurrentInteractable();
            }
        }
        else // If collides with nothing
        {
            DisableCurrentInteractable();
        }
    }

    private void SetNewCurrentInteractable(Interactable interactable)
    {
        currentInteractable = interactable;
        currentInteractable.EnableOutline();
        HUDManager.Instance.EnableInteractionText(currentInteractable.message);
    }

    private void DisableCurrentInteractable()
    {
        HUDManager.Instance.DisableInteractionText();
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}
