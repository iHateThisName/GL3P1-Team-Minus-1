using Assets.Scripts.Singleton;
using System.Collections;
using UnityEngine;

public class GameManager : PersistenSingleton<GameManager> {
    // References for easy access
    public PlayerMovement PlayerMovement;
    public Transform PlayerInteractTransform;
    public BreathingScript BreathingScript;

    // Game state variables
    private bool _isPlayerMovementEnabled = true; // Never access this directly, use the public property below
    public bool IsPlayerMovementEnabled {
        get => _isPlayerMovementEnabled;
        set {
            if (!value) {
                Rigidbody playerRigidBody = PlayerMovement?.GetRigidbody();
                if (playerRigidBody != null) {
                    playerRigidBody.linearVelocity = Vector3.zero;
                    playerRigidBody.angularVelocity = Vector3.zero;
                }
                this.PlayerMovement.OnMoveCanceled();
            }
            _isPlayerMovementEnabled = value;
        }
    }

    public int Money = 0;

    public bool startedGame = true;
    public bool startedGameFinished;
    public bool firstTreasureCollected;
    public bool firstTreasureFinished;
    public bool firstStoryCollected;
    public bool firstStoryFinished;

    public int GetHeldItemsValue() {
        if (PlayerInteractTransform == null) {
            Debug.LogWarning("PlayerInteractTransform is not assigned.");
            return 0;
        }


        Interactable[] heldItems = PlayerInteractTransform.GetComponentsInChildren<Interactable>();
        int totalValue = 0;
        foreach (Interactable item in heldItems) {
            totalValue += item.GetValueAmount();
        }
        return totalValue;
    }

    public void OnTestCall() {
        Debug.Log("Test call from GameManager");
    }

    public void UpMoveSpeed() {
        PlayerMovement.smallAcceleration += 3f;
        PlayerMovement.fastAcceleration += 4f;
        PlayerMovement.normalSpeed += 2f;
        PlayerMovement.fastSpeed += 2f;
        BreathingScript.upgradeValue -= 0.1f;
    }

    // Deletes the saved game state
    public void DeleteGameState() {
        PlayerPrefs.DeleteKey(ShopItemLookUp.PlayerShopItemsKey);
        ShopItemLookUp.Instance.ResetPlayerItems();
        GameManager.Instance.Money = 0;
    }

    public void TeleportPlayer(Vector3 newLocation) {
        Rigidbody playerRigidBody = this.PlayerMovement.GetRigidbody();

        // Directly set the player's position to the new location
        playerRigidBody.position = newLocation;

        // Reset velocities to prevent unwanted movement after teleportation
        playerRigidBody.linearVelocity = Vector3.zero;
        playerRigidBody.angularVelocity = Vector3.zero;
    }

    public void PlayerExitOcean() {
        this.PlayerMovement.isUnderWater = false;
        this.BreathingScript.DisableBreathing();
        this.BreathingScript.enabled = false;

        PlayerMovement.zoomedOut = false;

        FogController.Instance.DisableFogEffect();
    }

    public void PlayerEnterOcean() {
        this.PlayerMovement.isUnderWater = true;
        this.BreathingScript.enabled = true; // Making sure it is enabled

        BreathingScript.oxygenAmount = BreathingScript.intendedOxygen;

        PlayerMovement.zoomedOut = true;

        FogController.Instance.EnableFogEffect();
    }

    public void DropAllTreasure() {
        if (PlayerInteractTransform == null) {
            Debug.LogWarning("PlayerInteractTransform is not assigned.");
            return;
        }


        SunkenTreasure[] heldItems = PlayerInteractTransform.GetComponentsInChildren<SunkenTreasure>();

        foreach (SunkenTreasure item in heldItems) {
            item.DropTreasure();
            item.DetachFromPlayer();
            Debug.Log("Dropped treasure" + item);
        }
    }

    public IEnumerator PlayRespawnAnim() {
        DropAllTreasure();
        CheckPointManager.Instance.SetCurrentCheckPoint(7);
        CheckPointManager.Instance.UseCheckpoint();
        PlayerMovement.inCutscene = true;
        PlayerMovement.isUnderWater = false;

        yield return new WaitForSeconds(5f);
        PlayerMovement.rb.isKinematic = true;

        this.PlayerMovement.PlayRespawnAnim();
        yield return new WaitForSeconds(11.6f);
        PlayerMovement.inCutscene = false;
        PlayerMovement.rb.isKinematic = false;
        this.PlayerMovement.ResetAnims();
    }
}
