using UnityEngine;

public class SunkenTreasure : Interactable {
    [Tooltip("How much heavier the player becomes when picking this up")]
    [SerializeField] private float weight = 0.5f;

    [Tooltip("The transform of the treasure object that will be picked up")]
    [SerializeField] private Transform ObjectRootTransform; // The transform of the treasure object that will be picked up

    [SerializeField] private Rigidbody rb; // The Rigidbody of the treasure object

    private bool isCollected = false; // Whether the treasure has been collected
    private bool isPickedUp = false; // Whether the treasure is currently picked up by the player
    private void Awake() {
        if (this.ObjectRootTransform == null) {
            this.ObjectRootTransform = this.transform;
        }
        if (this.rb == null) {
            this.rb = GetComponent<Rigidbody>();
        }
    }

    // Will be called when the player interacts with this object
    public override void Interact() {
        if (!this.isCollected) {
            // Collects the treasure and also instantly picks it up
            this.isCollected = true;
            this.isPickedUp = true;
            this.rb.isKinematic = true; // Disable physics on the treasure

            GameManager.Instance.PlayerMovement.IncreaseWeight(this.weight);
            Debug.Log("Treasure collected! Player weight increased by " + this.weight);
        } else if (this.isPickedUp) {
            // Drops the treasure
            this.isPickedUp = false;
            this.rb.isKinematic = false; // Enable physics on the treasure

            GameManager.Instance.PlayerMovement.DecreaseWeight(this.weight);
            Debug.Log("Dropping Treasure");
        } else {
            // If the treasure is collected but not picked up, pick it up again
            // Or if the player drops it and wants to pick it up again
            this.isPickedUp = true;
            this.rb.isKinematic = true; // Disable physics on the treasure

            GameManager.Instance.PlayerMovement.IncreaseWeight(this.weight);
            Debug.Log("Picking up Treasure");
        }

        if (this.isPickedUp) {
            AttachToPlayer();
        } else {
            DetachFromPlayer();
        }
    }

    private void AttachToPlayer() {
        Transform playerTransform = GameManager.Instance.PlayerInteractTransform;
        this.ObjectRootTransform.SetParent(playerTransform);
    }

    private void DetachFromPlayer() {
        this.ObjectRootTransform.SetParent(null);
    }
}
