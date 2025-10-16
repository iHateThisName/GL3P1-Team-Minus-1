using Assets.Scripts.Singleton;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    // References for easy access
    public PlayerMovement PlayerMovement;
    public Transform PlayerInteractTransform;

    // Game state variables
    public bool IsPlayerMovmentEnabled = true;
    public int Money = 0;

    public int GetHeldItemsValue() {
        Interactable[] heldItems = PlayerInteractTransform.GetComponentsInChildren<Interactable>();
        int totalValue = 0;
        foreach (Interactable item in heldItems) {
            totalValue += item.GetValueAmount();
        }
        return totalValue;
    }
}
