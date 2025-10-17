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
            Debug.Log("TODO Not working properly because cant serialize Sprite to json");
            return;

            //string jsonPlayerItems = JsonConvert.SerializeObject(this.playersShopItems, formatting: Formatting.Indented);
            //Debug.Log($"Saving player shop items: {jsonPlayerItems}");
            //PlayerPrefs.SetString(PlayerShopItemsKey, jsonPlayerItems);
        }
    }

    private List<ShopItemData> LoadPlayerShopItems() {
        if (PlayerPrefs.HasKey(PlayerShopItemsKey)) {
            string jsonPlayerItems = PlayerPrefs.GetString(PlayerShopItemsKey);
            return new List<ShopItemData>(); // TODO Not working properly because cant serialize Sprite to json
            //List<ShopItemData> storedItems = JsonConvert.DeserializeObject<List<ShopItemData>>(jsonPlayerItems);
            //Debug.Log($"Loaded player shop items: {jsonPlayerItems}");
            //return storedItems ?? new List<ShopItemData>(); // Return empty list if deserialization fails

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
    public override string ToString() {
        return $"ItemType: {ItemType}, Price: {Price}";
    }
}

public enum EnumItemSprite {
    None, star, cross
}
