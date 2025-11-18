using UnityEngine;

public class ShopInteractable : Interactable {
    public override int GetValueAmount() => 0;

    public override void Interact() {
        Debug.Log("Interacting with Shop");
        GameManager.Instance.PlayerMovement.ResetAnims();
        GameSceneManager.Instance.ToggleShopMenu();
    }
}
