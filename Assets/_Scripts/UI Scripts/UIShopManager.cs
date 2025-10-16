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
        SpawnShopItem(100, null);
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
        UpdateSellButtonText();
    }

    private void SpawnShopItem(int cost, Sprite sprite) {
        GameObject shopItem = Instantiate(ShopItemPrefab, ShopItemsContainer);
        if (sprite != null) {
            shopItem.GetComponentInChildren<Image>().sprite = sprite;
        }
        shopItem.GetComponentInChildren<Text>().text = $"{cost}$";
    }

    private void UpdateSellButtonText() {
        this.sellButtonText.text = $"Sell {GameManager.Instance.GetHeldItemsValue()}$";
    }
}
