using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ShopInteractable : Interactable {

    private bool isInteracted = false;
    [SerializeField] private CinemachineCamera shopCamera;

    [SerializeField] private Transform shopSellButton;

    [Header("Flowers")]
    [SerializeField] Transform flowerOneOne;
    [SerializeField] Transform flowerOneTwo, flowerOneThree, flowerTwoOne, flowerTwoTwo, FlowerTwoThree;

    private Vector3 flowerDefaultValue;
    [SerializeField] private float flowerScaleMultiplier = 1.2f;

    [Header("Sold Out")]
    [SerializeField] private Image soldOutOneOne;
    [SerializeField] private Image soldOutOneTwo, soldOutOneThree, soldOutTwoOne, soldOutTwoTwo, soldOutTwoThree;

    private EnumShopGrid currentHoverdGrid = EnumShopGrid.None;
    public override int GetValueAmount() => 0;

    private IEnumerator Start() {
        this.flowerDefaultValue = this.flowerOneOne.localScale;
        this.shopSellButton.GetChild(0).gameObject.SetActive(false);
        this.shopSellButton.gameObject.SetActive(false);
        GameManager.Instance.ShopInteractable = this;

        yield return new WaitUntil(() => ShopItemLookUp.Instance.IsInitialized);
        // Update sold out images based on previously bought items
        foreach (ShopItemData item in ShopItemLookUp.Instance.GetAllPlayerItems()) {
            switch (item.ItemType) {
                case EnumItemSprite.suitUppgradeTier1:
                    this.soldOutOneOne.color = Color.white;
                    break;
                case EnumItemSprite.suitUppgradeTier2:
                    this.soldOutOneTwo.color = Color.white;
                    break;
                case EnumItemSprite.suitUppgradeTier3:
                    this.soldOutOneThree.color = Color.white;
                    break;
                case EnumItemSprite.suitUppgradeTier4:
                    this.soldOutTwoOne.color = Color.white;
                    break;
                case EnumItemSprite.suitUppgradeTier5:
                    this.soldOutTwoTwo.color = Color.white;
                    break;
                case EnumItemSprite.suitUppgradeTier6:
                    this.soldOutTwoThree.color = Color.white;
                    break;
            }
        }
    }
    public override void Interact() {
        Debug.Log("Interacting with Shop");

        if (this.isInteracted) {
            InteractShopItem();
            return;
        }

        this.isInteracted = !this.isInteracted;
        UpdateCamera();

        if (this.isInteracted) {
            this.shopSellButton.gameObject.SetActive(true);
            OnShopOpened();
        } else {

            OnShopClosed();
        }

        GameManager.Instance.PlayerMovement.ResetAnims();
    }

    private void InteractShopItem() {

        List<ShopItemData> boughtItems = ShopItemLookUp.Instance.GetAllPlayerItems();

        switch (this.currentHoverdGrid) {
            case EnumShopGrid.SellButton:
                int collectedValueAmount = GameManager.Instance.GetHeldItemsValue();

                // If nothing to sell, exit
                if (collectedValueAmount <= 0) break;
                AudioManager.Instance.sellTreasureSound.Play();

                // Add money to player
                GameManager.Instance.Money += collectedValueAmount;

                // Destroy all held items, because they are sold
                Interactable[] heldItems = GameManager.Instance.PlayerInteractTransform.GetComponentsInChildren<Interactable>();
                foreach (Interactable item in heldItems) {
                    Destroy(item.gameObject);
                }

                GameManager.Instance.BreathingScript.weightValue = 0f;
                GameManager.Instance.UIShopManager.UpdateMoney();
                break;

            case EnumShopGrid.OneOne:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier1)) break;

                // Attempt to buy item
                ShopItemData suitOne = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier1);
                if (GameManager.Instance.UIShopManager.BuyItem(suitOne)) {
                    soldOutOneOne.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;

            case EnumShopGrid.OneTwo:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier2)) break;

                // Attempt to buy item
                ShopItemData suitTwo = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier2);
                if (GameManager.Instance.UIShopManager.BuyItem(suitTwo)) {
                    soldOutOneTwo.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;

            case EnumShopGrid.OneThree:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier3)) break;

                // Attempt to buy item
                ShopItemData suitThree = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier3);
                if (GameManager.Instance.UIShopManager.BuyItem(suitThree)) {
                    soldOutOneThree.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;

            case EnumShopGrid.TwoOne:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier4)) break;

                // Attempt to buy item
                ShopItemData suitFour = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier4);
                if (GameManager.Instance.UIShopManager.BuyItem(suitFour)) {
                    soldOutTwoOne.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;

            case EnumShopGrid.TwoTwo:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier5)) break;

                // Attempt to buy item
                ShopItemData suitFive = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier5);
                if (GameManager.Instance.UIShopManager.BuyItem(suitFive)) {
                    soldOutTwoTwo.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;

            case EnumShopGrid.TwoThree:
                // Check if already bought
                if (boughtItems.Exists(item => item.ItemType == EnumItemSprite.suitUppgradeTier6)) break;

                // Attempt to buy item
                ShopItemData suitSix = ShopItemLookUp.Instance.GetShopItemData(EnumItemSprite.suitUppgradeTier6);
                if (GameManager.Instance.UIShopManager.BuyItem(suitSix)) {
                    soldOutTwoThree.color = Color.white;
                    AudioManager.Instance.shopPurchaseSound.Play();
                }
                break;
        }
    }

    private void OnShopOpened() {
        GameManager.Instance.IsPlayerMovementEnabled = false;
        HoverFlower(EnumShopGrid.OneOne);
        AudioManager.Instance.shopEnterAndExit.Play();
    }

    public void OnShopClosed() {
        UnhoverFlower(this.currentHoverdGrid);
        this.currentHoverdGrid = EnumShopGrid.None;
        this.shopSellButton.gameObject.SetActive(false);
        this.isInteracted = false;
        UpdateCamera();
        GameManager.Instance.IsPlayerMovementEnabled = true;
        AudioManager.Instance.shopEnterAndExit.Play();
    }


    private void UpdateCamera() {
        Camera mainCamera = Camera.main;
        int playerLayer = LayerMask.NameToLayer("Player");
        if (this.isInteracted) {
            mainCamera.cullingMask &= ~(1 << playerLayer); // Hide player layer
            this.shopCamera.Priority = 1;
            StartCoroutine(ToggleShopUI());
        } else {
            mainCamera.cullingMask |= (1 << playerLayer); // Show player layer
            this.shopCamera.Priority = -1;
            //GameSceneManager.Instance.ToggleShopMenu();
        }
    }

    public void Navigate(Vector2 direction) {

        if (direction.x < 0) {
            // The player moved left on the shop grid

            switch (this.currentHoverdGrid) {
                case EnumShopGrid.OneOne:
                    HoverFlower(EnumShopGrid.SellButton);
                    break;
                case EnumShopGrid.OneTwo:
                    HoverFlower(EnumShopGrid.OneOne);
                    break;
                case EnumShopGrid.OneThree:
                    HoverFlower(EnumShopGrid.OneTwo);
                    break;
                case EnumShopGrid.TwoOne:
                    HoverFlower(EnumShopGrid.SellButton);
                    break;
                case EnumShopGrid.TwoTwo:
                    HoverFlower(EnumShopGrid.TwoOne);
                    break;
                case EnumShopGrid.TwoThree:
                    HoverFlower(EnumShopGrid.TwoTwo);
                    break;
            }
        } else if (direction.x > 0) {
            // The player moved right on the shop grid

            switch (this.currentHoverdGrid) {
                case EnumShopGrid.SellButton:
                    HoverFlower(EnumShopGrid.OneOne);
                    break;
                case EnumShopGrid.OneOne:
                    HoverFlower(EnumShopGrid.OneTwo);
                    break;
                case EnumShopGrid.OneTwo:
                    HoverFlower(EnumShopGrid.OneThree);
                    break;
                case EnumShopGrid.TwoOne:
                    HoverFlower(EnumShopGrid.TwoTwo);
                    break;
                case EnumShopGrid.TwoTwo:
                    HoverFlower(EnumShopGrid.TwoThree);
                    break;
            }
        }

        if (direction.y > 0) {
            // The player moved up on the shop grid
            switch (this.currentHoverdGrid) {
                case EnumShopGrid.TwoOne:
                    HoverFlower(EnumShopGrid.OneOne);
                    break;
                case EnumShopGrid.TwoTwo:
                    HoverFlower(EnumShopGrid.OneTwo);
                    break;
                case EnumShopGrid.TwoThree:
                    HoverFlower(EnumShopGrid.OneThree);
                    break;
            }
        } else if (direction.y < 0) {
            // The player moved down on the shop grid
            switch (this.currentHoverdGrid) {
                case EnumShopGrid.OneOne:
                    HoverFlower(EnumShopGrid.TwoOne);
                    break;
                case EnumShopGrid.OneTwo:
                    HoverFlower(EnumShopGrid.TwoTwo);
                    break;
                case EnumShopGrid.OneThree:
                    HoverFlower(EnumShopGrid.TwoThree);
                    break;
            }
        }
    }

    private void HoverFlower(EnumShopGrid grid) {
        if (this.currentHoverdGrid == grid) return; // Already hovered

        UnhoverFlower(this.currentHoverdGrid); // Unhover previous

        switch (grid) {
            case EnumShopGrid.SellButton:
                this.shopSellButton.GetChild(0).gameObject.SetActive(true);
                break;
            case EnumShopGrid.OneOne:
                this.flowerOneOne.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.flowerOneOne.GetChild(0).gameObject.SetActive(true);
                break;

            case EnumShopGrid.OneTwo:
                this.flowerOneTwo.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.flowerOneTwo.GetChild(0).gameObject.SetActive(true);
                break;

            case EnumShopGrid.OneThree:
                this.flowerOneThree.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.flowerOneThree.GetChild(0).gameObject.SetActive(true);
                break;

            case EnumShopGrid.TwoOne:
                this.flowerTwoOne.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.flowerTwoOne.GetChild(0).gameObject.SetActive(true);
                break;

            case EnumShopGrid.TwoTwo:
                this.flowerTwoTwo.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.flowerTwoTwo.GetChild(0).gameObject.SetActive(true);
                break;

            case EnumShopGrid.TwoThree:
                this.FlowerTwoThree.localScale = this.flowerDefaultValue * this.flowerScaleMultiplier;
                this.FlowerTwoThree.GetChild(0).gameObject.SetActive(true);
                break;
        }
        this.currentHoverdGrid = grid; // Update current hovered grid
    }

    private void UnhoverFlower(EnumShopGrid grid) {
        switch (grid) {
            case EnumShopGrid.SellButton:
                this.shopSellButton.GetChild(0).gameObject.SetActive(false);
                break;
            case EnumShopGrid.OneOne:
                this.flowerOneOne.localScale = this.flowerDefaultValue;
                this.flowerOneOne.GetChild(0).gameObject.SetActive(false);
                break;

            case EnumShopGrid.OneTwo:
                this.flowerOneTwo.localScale = this.flowerDefaultValue;
                this.flowerOneTwo.GetChild(0).gameObject.SetActive(false);
                break;

            case EnumShopGrid.OneThree:
                this.flowerOneThree.localScale = this.flowerDefaultValue;
                this.flowerOneThree.GetChild(0).gameObject.SetActive(false);
                break;

            case EnumShopGrid.TwoOne:
                this.flowerTwoOne.localScale = this.flowerDefaultValue;
                this.flowerTwoOne.GetChild(0).gameObject.SetActive(false);
                break;

            case EnumShopGrid.TwoTwo:
                this.flowerTwoTwo.localScale = this.flowerDefaultValue;
                this.flowerTwoTwo.GetChild(0).gameObject.SetActive(false);
                break;

            case EnumShopGrid.TwoThree:
                this.FlowerTwoThree.localScale = this.flowerDefaultValue;
                this.FlowerTwoThree.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }

    private IEnumerator ToggleShopUI() {
        yield return new WaitForSecondsRealtime(2f);
        GameSceneManager.Instance.ToggleShopMenu();
    }
}
