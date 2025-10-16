using UnityEngine;

public class ShopInteractable : Interactable {

    public override void Interact() {
        Debug.Log("Interacting with Shop");
        GameSceneManager.Instance.ToggleShopMenu();
    }
}
