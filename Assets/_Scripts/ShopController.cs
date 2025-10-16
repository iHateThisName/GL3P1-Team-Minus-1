using UnityEngine;

public class ShopController : Interactable {

    public override void Interact() {
        Debug.Log("Interacting with Shop");
        GameSceneManager.Instance.ToggleShopMenu();
    }
}
