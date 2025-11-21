using Assets.Scripts.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ShopItemLookUp : PersistenSingleton<ShopItemLookUp> {


    [SerializeField] private List<ShopItemData> allShopItems = new List<ShopItemData>();
    [SerializeField] private List<ShopItemData> playersShopItems = new List<ShopItemData>();

    public static readonly string PlayerShopItemsKey = "PlayerShopItems";
    //public Dictionary<EnumItemSprite, ShopItemData> ShopItemDictionary { get; private set; } = new Dictionary<EnumItemSprite, ShopItemData>();
    public bool IsInitialized { get; private set; } = false;
    private void Start() {
        this.allShopItems = this.allShopItems.OrderBy(item => item.Price).ToList();

        // Use player prefs to load previously bought items
        this.playersShopItems = LoadPlayerShopItems();

        foreach (var item in playersShopItems) {
            Debug.Log(item.ToString());
        }

        // Mark as initialized
        this.IsInitialized = true; // Might be redundant
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        Debug.Log("ShopItemLookUp OnDestroy called.");
        if (ShopItemLookUp.Instance == this) { // Make sure this is the active instance
            // TODO Not working properly because cant serialize Sprite
            List<ShopItemDataDTO> playerShopItemsDTO = new List<ShopItemDataDTO>();

            this.playersShopItems.ForEach(item => {
                playerShopItemsDTO.Add(item.SeriliazeToDTO());
            });

            string jsonPlayerItems = JsonUtility.ToJson(new PlayerItemsWrapper(playerShopItemsDTO, GameManager.Instance.Money), prettyPrint: true);
            PlayerPrefs.SetString(PlayerShopItemsKey, jsonPlayerItems);
        }
    }

    private void OnEnable() {
        if (ShopItemLookUp.Instance == this) {
            this.playersShopItems = LoadPlayerShopItems();
        }
    }

    public List<ShopItemData> GetAllPlayerItems() => this.playersShopItems;

    private List<ShopItemData> LoadPlayerShopItems() {
        if (PlayerPrefs.HasKey(PlayerShopItemsKey)) {
            string jsonPlayerItems = PlayerPrefs.GetString(PlayerShopItemsKey);
            //List<ShopItemData> storedItems = JsonConvert.DeserializeObject<List<ShopItemData>>(jsonPlayerItems);
            PlayerItemsWrapper wrapper = JsonUtility.FromJson<PlayerItemsWrapper>(jsonPlayerItems);

            // Restore player's money
            GameManager.Instance.Money = wrapper.Money;

            // Restore player's items
            List<ShopItemData> storedItems = new List<ShopItemData>();
            wrapper.PlayerItems.ForEach(dto => {
                // Find the corresponding sprite from allShopItems
                Sprite itemSprite = null;

                // Search for the sprite in allShopItems
                for (int i = 0; i < this.allShopItems.Count; i++) {
                    if (this.allShopItems[i].ItemType == dto.ItemType && this.allShopItems[i].Sprite != null) {
                        itemSprite = this.allShopItems[i].Sprite;
                        break;
                    }
                }

                // I am allowing the sprite to be null on purpose.
                storedItems.Add(dto.DeSeriliazeFromDTO(itemSprite));

                if (itemSprite == null) {
                    Debug.LogWarning($"Sprite for item type {dto.ItemType} not found in allShopItems.");
                }
            });

            return storedItems;

        } else {
            return new List<ShopItemData>();
        }
    }

    public List<ShopItemData> GetAviableShopItems() {
        List<ShopItemData> aviableItems = new List<ShopItemData>();
        foreach (ShopItemData item in this.allShopItems) {
            if (!this.playersShopItems.Contains(item)) {
                aviableItems.Add(item);
            }
        }

        return aviableItems;
    }
    public List<ShopItemData> GetBoughtShopItems() => this.playersShopItems;
    public void RegisterShopItem(ShopItemData item) {
        if (!item.IsValide) {
            Debug.LogWarning("Attempted to add null item to bought shop items.");
        } else {
            this.playersShopItems.Add(item);
            if (item.ItemType.ToString().Contains("Uppgrade")) {
                GameManager.Instance.UpMoveSpeed();
            }
        }
    }

    public List<ShopItemData> GetAllPlayerUppgrades() {
        List<ShopItemData> uppgrades = new List<ShopItemData>();

        foreach (var item in this.playersShopItems) {
            if (item.ItemType.ToString().Contains("Uppgrade")) {
                uppgrades.Add(item);
            }
        }

        return uppgrades;
    }

    public void ResetPlayerItems() {
        this.playersShopItems.Clear();
    }
}

[System.Serializable]
public struct ShopItemData {
    public EnumItemSprite ItemType;
    public Sprite Sprite;
    public int Price;
    public bool IsValide => ItemType != EnumItemSprite.None && Sprite != null && Price >= 0;

    public ShopItemData(EnumItemSprite itemType, Sprite sprite, int price) {
        ItemType = itemType;
        Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
        Price = price;
    }

    public ShopItemDataDTO SeriliazeToDTO() {
        return new ShopItemDataDTO(this.ItemType, this.Price);
    }
    public override string ToString() {
        return $"ItemType: {ItemType}, Price: {Price}";
    }
}

[System.Serializable]
public class ShopItemDataDTO {
    public EnumItemSprite ItemType;
    public int Price;

    public ShopItemDataDTO(EnumItemSprite itemType, int price) {
        ItemType = itemType;
        Price = price;
    }

    public ShopItemData DeSeriliazeFromDTO(Sprite sprite) {
        return new ShopItemData(this.ItemType, sprite, this.Price);
    }
}
[System.Serializable]
public class PlayerItemsWrapper {
    public List<ShopItemDataDTO> PlayerItems;
    public int Money;
    public PlayerItemsWrapper(List<ShopItemDataDTO> items, int money) {
        PlayerItems = items;
        Money = money;
    }
}

public enum EnumItemSprite {
    None, star, cross, suitUppgradeTier1, suitUppgradeTier2, suitUppgradeTier3, suitUppgradeTier4, suitUppgradeTier5, suitUppgradeTier6
}
