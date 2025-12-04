using UnityEngine;
#if !UNITY_WEBGL
using Xasu.HighLevel;
#endif

public class SunkenTreasure : Interactable {
    [SerializeField] private int value = 100; // The value of the treasure

    [Tooltip("How much heavier the player becomes when picking this up")]
    [SerializeField] private float weight = 0.5f;

    [Tooltip("The transform of the treasure object that will be picked up")]
    [SerializeField] private Transform ObjectRootTransform; // The transform of the treasure object that will be picked up

    [SerializeField] private Rigidbody rb; // The Rigidbody of the treasure object

    [SerializeField] private Transform ModelTransform; // The model of the treasure object

    private bool isCollected = false; // Whether the treasure has been collected
    private bool isPickedUp = false; // Whether the treasure is currently picked up by the player

    [SerializeField] private bool isStoryTreasure = false; // Whether the treasure is a story treasure
    [SerializeField] private bool isArtefact = false;

    [SerializeField] private StoryTreasureScript storyTreasureScript;
    [SerializeField] private BreathingScript breathingScript;

    [SerializeField] private string treasureMessage;

    [SerializeField] private int treasureNum = 0;

    [SerializeField]
    private SeaShellScript seaShell;

    [SerializeField]
    private bool isInShell = false;

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
            CollectTreasure();
        } else if (this.isPickedUp) {
            DropTreasure();
        } else {
            PickUpTreasure();
        }

        if (this.isPickedUp) {
            AttachToPlayer();
            this.ModelTransform.gameObject.SetActive(false);
        } else {
            this.ModelTransform.gameObject.SetActive(true);
            DetachFromPlayer();
        }
    }

    public override int GetValueAmount() {
        return this.value;
    }

    private void PickUpTreasure() {
        // If the treasure is collected but not picked up, pick it up again
        // Or if the player drops it and wants to pick it up again
        this.isPickedUp = true;
        //this.rb.isKinematic = true; // Disable physics on the treasure

        GameManager.Instance.BreathingScript.IncreaseWeight(this.weight);
        Debug.Log("Picking up Treasure");
    }

    public void DropTreasure() {
        // Drops the treasure
        this.isPickedUp = false;
        //this.rb.isKinematic = false; // Enable physics on the treasure

        GameManager.Instance.BreathingScript.DecreaseWeight(this.weight);
        Debug.Log("Dropping Treasure");
    }

    private void CollectTreasure() {
#if !UNITY_WEBGL
        GameObjectTracker.Instance.Used(gameObject.name).WithResultExtensions(new System.Collections.Generic.Dictionary<string, object>
        {
            { "http://isStoryTreasure", isStoryTreasure },
            { "http://isArtefact", isArtefact },
            { "http://isNormalTreasure", !isArtefact && !isStoryTreasure }
        });
#endif
        if (!GameManager.Instance.firstTreasureCollected) {
            GameManager.Instance.firstTreasureCollected = true;
        }
        // Collects the treasure and also instantly picks it up
        this.isCollected = true;
        this.isPickedUp = true;
        this.rb.isKinematic = true; // Disable physics on the treasure

        GameManager.Instance.BreathingScript.IncreaseWeight(this.weight);
        Debug.Log("Treasure collected! Player weight increased by " + this.weight);

        if (isStoryTreasure || isArtefact) {
            if (isStoryTreasure) {
                if (!GameManager.Instance.firstStoryCollected) {
                    GameManager.Instance.firstStoryCollected = true;
                } else if (!GameManager.Instance.secondStoryCollected) {
                    GameManager.Instance.secondStoryCollected = true;
                } else if (!GameManager.Instance.thirdStoryCollected) {
                    GameManager.Instance.thirdStoryCollected = true;
                } else if (!GameManager.Instance.fourthStoryCollected) {
                    GameManager.Instance.fourthStoryCollected = true;
                }
            }
            GameManager.Instance.PlayerMovement.ResetMomentum();
            GameManager.Instance.PlayerMovement.enabled = false;
            breathingScript.enabled = false;
            storyTreasureScript.DisplayTreasureScreen(treasureNum, treasureMessage, isStoryTreasure);
        }

        if (isInShell && seaShell != null) {
            seaShell.CloseShell();
        }
    }

    private void AttachToPlayer() {
        this.rb.isKinematic = true;
        Transform playerTransform = GameManager.Instance.PlayerInteractTransform;
        this.ObjectRootTransform.SetParent(playerTransform);
    }

    public void DetachFromPlayer() {
        this.ObjectRootTransform.SetParent(null);
        this.ModelTransform.gameObject.SetActive(true);
        this.rb.isKinematic = false; // Enable physics on the treasure
    }
}
