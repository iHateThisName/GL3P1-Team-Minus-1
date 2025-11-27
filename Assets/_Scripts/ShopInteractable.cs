using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ShopInteractable : Interactable {

    private bool isInteracted = false;
    [SerializeField] private CinemachineCamera shopCamera;

    [SerializeField] private Transform shopSellButton;

    [Header("Flowers")]
    [SerializeField] Transform flowerOneOne;
    [SerializeField] Transform flowerOneTwo, flowerOneThree, flowerTwoOne, flowerTwoTwo, FlowerTwoThree;

    private Vector3 flowerDefaultValue;
    [SerializeField] private float flowerScaleMultiplier = 1.2f;

    private EnumShopGrid currentHoverdGrid = EnumShopGrid.None;
    public override int GetValueAmount() => 0;


    private void Start() {
        this.flowerDefaultValue = this.flowerOneOne.localScale;
        GameManager.Instance.ShopInteractable = this;
    }
    public override void Interact() {
        Debug.Log("Interacting with Shop");

        this.isInteracted = !this.isInteracted;

        UpdateCamera();

        if (this.isInteracted) {
            this.shopSellButton.gameObject.SetActive(true);
            OnShopOpened();
        } else {
            this.shopSellButton.gameObject.SetActive(false);
            OnShopClosed();
        }

        GameManager.Instance.PlayerMovement.ResetAnims();
    }
    private void OnShopOpened() {
        GameManager.Instance.IsPlayerMovementEnabled = false;
        HoverFlower(EnumShopGrid.OneOne);
    }

    private void OnShopClosed() {
        GameManager.Instance.IsPlayerMovementEnabled = true;
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
            GameSceneManager.Instance.ToggleShopMenu();
        }
    }

    public void Navigate(Vector2 direction) {

        if (direction.x < 0) {
            // The player moved left on the shop grid

            switch (this.currentHoverdGrid) {
                case EnumShopGrid.OneTwo:
                    HoverFlower(EnumShopGrid.OneOne);
                    break;
                case EnumShopGrid.OneThree:
                    HoverFlower(EnumShopGrid.OneTwo);
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
