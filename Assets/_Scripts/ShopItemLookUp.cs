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

    protected override void Awake() {
        base.Awake();
        this.allShopItems = this.allShopItems.OrderBy(item => item.Price).ToList();

        // Initialize the shop item dictionary based on inspector data from the list
        //this.allShopItems.ForEach(listItem => {
        //    if (!ShopItemDictionary.ContainsKey(listItem.ItemType)) {
        //        ShopItemDictionary.Add(listItem.ItemType, listItem);
        //    } else {
        //        Debug.LogWarning($"Duplicate listItem type {listItem.ItemType} in shop items list.");
        //    }
        //});


        // Use player prefs to load previously bought items
        this.playersShopItems = LoadPlayerShopItems();

        foreach (var item in playersShopItems) {
            Debug.Log(item.ToString());
        }

        // Mark as initialized
        this.IsInitialized = true; // Might be redundant since its happening in Awake()
    }

    private void OnDestroy() {
        Debug.Log("ShopItemLookUp OnDestroy called.");
        if (this.playersShopItems.Count > 0 && ShopItemLookUp.Instance == this) { // Make sure this is the active instance
            // TODO Not working properly because cant serialize Sprite
            List<ShopItemDataDTO> playerShopItemsDTO = new List<ShopItemDataDTO>();

            this.playersShopItems.ForEach(item => {
                playerShopItemsDTO.Add(item.SeriliazeToDTO());
            });

            string jsonPlayerItems = JsonUtility.ToJson(new PlayerItemsWrapper(playerShopItemsDTO), prettyPrint: true);
            PlayerPrefs.SetString(PlayerShopItemsKey, jsonPlayerItems);
        }
    }

    private void OnEnable() {
        if (ShopItemLookUp.Instance == this) {
            this.playersShopItems = LoadPlayerShopItems();
        }
    }

    private List<ShopItemData> LoadPlayerShopItems() {
        if (PlayerPrefs.HasKey(PlayerShopItemsKey)) {
            string jsonPlayerItems = PlayerPrefs.GetString(PlayerShopItemsKey);
            //List<ShopItemData> storedItems = JsonConvert.DeserializeObject<List<ShopItemData>>(jsonPlayerItems);
            PlayerItemsWrapper wrapper = JsonUtility.FromJson<PlayerItemsWrapper>(jsonPlayerItems);

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
        if (item == null) {
            Debug.LogWarning("Attempted to add null item to bought shop items.");
        } else {
            this.playersShopItems.Add(item);
        }
    }


}

[System.Serializable]
public class ShopItemData {
    public EnumItemSprite ItemType;
    public Sprite Sprite;
    public int Price;

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
    public PlayerItemsWrapper(List<ShopItemDataDTO> items) {
        PlayerItems = items;
    }
}

public enum EnumItemSprite {
    None, star, cross, suitUppgradeTier1
}
