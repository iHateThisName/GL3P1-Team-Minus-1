using Assets.Scripts.Singleton;
using UnityEngine;

public class GameManager : PersistenSingleton<GameManager> {
    // References for easy access
    public PlayerMovement PlayerMovement;
    public Transform PlayerInteractTransform;
    public BreathingScript BreathingScript;

    // Game state variables
    public bool IsPlayerMovementEnabled = true;
    public int Money = 0;

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
        PlayerMovement.smallAcceleration += 3;
        PlayerMovement.fastAcceleration += 4;
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
}
