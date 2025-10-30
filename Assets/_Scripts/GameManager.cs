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

    // Deletes the saved game state
    public void DeleteGameState() {
        PlayerPrefs.DeleteKey(ShopItemLookUp.PlayerShopItemsKey);
        ShopItemLookUp.Instance.ResetPlayerItems();
        GameManager.Instance.Money = 0;
    }
}
