using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    
    public void OnInteract(InputAction.CallbackContext context)
{
    Debug.Log($"OnInteract called: phase={context.phase}, interactableInRange={interactableInRange != null}");
    
    if (context.performed)
        {
            Debug.Log("Interact button performed");
            if (interactableInRange != null)
            {
                Debug.Log($"Attempting to interact with {interactableInRange.GetType().Name}");
                interactableInRange.Interact();
            }
            else
            {
                Debug.Log("No interactable object in range");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Trigger entered with {other.gameObject.name}");
        
        if (other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            Debug.Log($"Found IInteractable on {other.gameObject.name}");
            
            if (interactable.CanInteract())
            {
                Debug.Log($"Setting interactableInRange to {other.gameObject.name}");
                interactableInRange = interactable;
            }
            else
            {
                Debug.Log($"IInteractable on {other.gameObject.name} cannot be interacted with");
            }
        }
        else
        {
            Debug.Log($"No IInteractable component on {other.gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Trigger exited with {other.gameObject.name}");
        
        if (other.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (interactable == interactableInRange)
            {
                Debug.Log($"Clearing interactableInRange (was {other.gameObject.name})");
                interactableInRange = null;
            }
        }
    }

    // Add this method to check the state on demand
    public void LogInteractableStatus()
    {
        Debug.Log($"Current interactableInRange: {(interactableInRange != null ? interactableInRange.GetType().Name : "null")}");
    }
}
