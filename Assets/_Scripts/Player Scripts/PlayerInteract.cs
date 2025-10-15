using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer; // Trying to use layers

    private void OnEnable() {
        this.playerInput.actions["Interact"].performed += OnInteract;
    }

    private void OnDisable() {
        this.playerInput.actions["Interact"].performed -= OnInteract;
    }

    private void Awake() {
        GameManager.Instance.PlayerInteractTransform = this.transform; // Register this transform with the GameManager
    }

    private void OnInteract(InputAction.CallbackContext context) {
        // Find nearby interactables
        Collider[] hits = Physics.OverlapSphere(transform.position, this.interactRange, this.interactableLayer);
        foreach (Collider hit in hits) {
            // Check the parent first
            Interactable interactable = hit.GetComponentInParent<Interactable>();
            if (interactable != null) {
                interactable.Interact();
                break; // Interact with the first valid interactable found
            } else {
                // If not found in parent, check directly on the collider
                interactable = hit.GetComponent<Interactable>();
                if (interactable != null) {
                    interactable.Interact();
                    break;
                }
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, this.interactRange);
    }
}
