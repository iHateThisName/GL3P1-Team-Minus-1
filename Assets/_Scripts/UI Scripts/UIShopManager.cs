using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIShopManager : MonoBehaviour {
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_Text sellButtonText;
    [SerializeField] private Transform ShopItemsContainer;

    [SerializeField] private GameObject ShopItemPrefab;

    [SerializeField] private TMP_Text moneyText;

    private int collectedValueAmount = 0;

    [SerializeField] private InputActionReference InputMoveAction;

    private void OnEnable() {
        sellButton.onClick.AddListener(OnSellButton);
        InputMoveAction.action.performed += OnNavigateStoreItems;
    }


    private void OnDisable() {
        this.sellButton.onClick.RemoveListener(OnSellButton);
        InputMoveAction.action.performed -= OnNavigateStoreItems;
    }
    private void OnNavigateStoreItems(InputAction.CallbackContext context) {
        Vector2 inputVector = context.ReadValue<Vector2>();
        GameManager.Instance.ShopInteractable.Navigate(inputVector);
    }

    private void Start() {
        UpdateSellButtonText();
        UpdateMoney();
        GameManager.Instance.UIShopManager = this;
    }

    public void OnSellButton() {
        collectedValueAmount = GameManager.Instance.GetHeldItemsValue();
        GameManager.Instance.Money += collectedValueAmount;
        Debug.Log($"Sold items for {collectedValueAmount}$");
        // Destroy all held items
        Interactable[] heldItems = GameManager.Instance.PlayerInteractTransform.GetComponentsInChildren<Interactable>();
        foreach (Interactable item in heldItems) {
            Destroy(item.gameObject);
        }
        //this.sellButtonText.text = $"Sell 0";
        GameManager.Instance.BreathingScript.weightValue = 0f;
        UpdateMoney();
    }

    private void SpawnShopItem(ShopItemData itemData) {
        GameObject shopItem = Instantiate(ShopItemPrefab, ShopItemsContainer);
        if (itemData.Sprite != null) {
            //shopItem.GetComponentInChildren<Image>().sprite = sprite; There are multiple images, so we need to be specific
            shopItem.transform.Find("Model/ItemImage").GetComponent<Image>().sprite = itemData.Sprite;
            Debug.Log(itemData.Sprite.name);
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

        if (BuyItem(itemData)) {
            // Successfully bought the item
            Destroy(shopItem); // Remove the item from the shop UI
            Debug.Log($"Purchased item for {itemData.Price}$");

        } else {
            // Failed to buy the item
            Debug.Log("Not enough money to purchase this item.");

        }
    }

    public bool BuyItem(ShopItemData itemData) {
        // Enough money to buy the item
        if (GameManager.Instance.Money >= itemData.Price) {
            GameManager.Instance.Money -= itemData.Price;
            // TODO Do Animation with sound etc.
            // TODO Give an effect to the player

            // Store the purchased item in the ShopItemLookUp
            if (itemData.IsValide) {
                ShopItemLookUp.Instance.RegisterShopItem(itemData);
            }

            if (itemData.ItemType == EnumItemSprite.suitUppgradeTier1) {
                GameManager.Instance.firstSuitUpgrade = true;
            }

            UpdateMoney();
            return true;
        } else {
            return false;
        }
    }

    public void OnShopItemClicked(GameObject shopItem) {
        // Create an invalid ShopItemData and forward to the main handler
        ShopItemData invalidItem = new ShopItemData { };
        OnShopItemClicked(invalidItem, shopItem);
    }

    private void UpdateSellButtonText() {
        this.sellButtonText.text = $"Sell {GameManager.Instance.GetHeldItemsValue()}$";
    }

    public void OnExitButton() => GameSceneManager.Instance.ToggleShopMenu();
    public void OnCheckpointButton(int checkNum) {
        CheckPointManager.Instance.SetCurrentCheckPoint(checkNum);
        CheckPointManager.Instance.UseCheckpoint();
        OnExitButton();
    }

    public void UpdateMoney() {

        string moneyString = GameManager.Instance.Money.ToString();
        moneyString = FormatMoneyString(moneyString);

        moneyText.text = "$" + moneyString;
    }

    private static string FormatMoneyString(string moneyString) {
        if (moneyString.Length > 3) {
            moneyString = moneyString.Insert(moneyString.Length - 3, " ");
            if (moneyString.Length > 7) { // 6 + 1 space
                moneyString = moneyString.Insert(moneyString.Length - 7, " ");
            }
        }

        return moneyString;
    }
}
