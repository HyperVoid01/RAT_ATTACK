using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach;
    public Interactable currentInteractable;

    private void Update()
    {
        CheckInteraction();
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable)
        {
            currentInteractable.Interact();
        }
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
