using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour {
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text sellButtonText;
    [SerializeField] private Transform ShopItemsContainer;

    [SerializeField] private GameObject ShopItemPrefab;

    private int collectedValueAmount = 0;

    private void OnEnable() {
        sellButton.onClick.AddListener(OnSellButton);
    }

    private void OnDisable() {
        this.sellButton.onClick.RemoveListener(OnSellButton);
    }

    private void Start() {
        UpdateSellButtonText();

        int currentDisplayCount = ShopItemsContainer.childCount;
        List<ShopItemData> allShopItems = ShopItemLookUp.Instance.GetAviableShopItems();

        for (int i = 0; i < 3 - currentDisplayCount; i++) {
            SpawnShopItem(allShopItems[i]);
        }
    }

    private void OnSellButton() {
        collectedValueAmount = GameManager.Instance.GetHeldItemsValue();
        GameManager.Instance.Money += collectedValueAmount;
        Debug.Log($"Sold items for {collectedValueAmount}$");
        // Destroy all held items
        Interactable[] heldItems = GameManager.Instance.PlayerInteractTransform.GetComponentsInChildren<Interactable>();
        foreach (Interactable item in heldItems) {
            Destroy(item.gameObject);
        }
        this.sellButtonText.text = $"Sell 0$";
    }

    private void SpawnShopItem(ShopItemData itemData) {
        GameObject shopItem = Instantiate(ShopItemPrefab, ShopItemsContainer);
        if (itemData.Sprite != null) {
            //shopItem.GetComponentInChildren<Image>().sprite = sprite; There are multiple images, so we need to be specific
            shopItem.transform.Find("Model/ItemImage").GetComponent<Image>().sprite = itemData.Sprite;
        }
        Transform textTransfrom = shopItem.transform.Find("Model/TopInfoContainer/PriceText");
        textTransfrom.GetComponent<TMP_Text>().text = $"{itemData.Price}$";

        // Button 
        Button shopItemButton = shopItem.GetComponent<Button>();

        shopItemButton.onClick.AddListener(() => OnShopItemClicked(itemData, shopItem));
    }

    private void OnShopItemClicked(ShopItemData itemData, GameObject shopItem) {
        Transform textTransfrom = shopItem.transform.Find("Model/TopInfoContainer/PriceText");
        int price = int.Parse(textTransfrom.GetComponent<TMP_Text>().text.Replace("$", ""));

        // Enough money to buy the item
        if (GameManager.Instance.Money >= price) {
            GameManager.Instance.Money -= price;
            // TODO Do Animation with sound etc.
            // TODO Give an effect to the player

            // Store the purchased item in the ShopItemLookUp
            if (itemData != null) {
                ShopItemLookUp.Instance.RegisterShopItem(itemData);
            }

            Debug.Log($"Purchased item for {price}$");
            Destroy(shopItem); // Remove the item from the shop UI
        } else {
            Debug.Log("Not enough money to purchase this item.");
        }
    }

    public void OnShopItemClicked(GameObject shopItem) => OnShopItemClicked(null, shopItem);

    private void UpdateSellButtonText() {
        this.sellButtonText.text = $"Sell {GameManager.Instance.GetHeldItemsValue()}$";
    }

    public void OnExitButton() => GameSceneManager.Instance.ToggleShopMenu();
}
