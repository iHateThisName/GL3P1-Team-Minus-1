using Assets.Scripts.Singleton;
using System.Collections;
using UnityEngine;

public class GameManager : PersistenSingleton<GameManager> {
    // References for easy access
    public PlayerMovement PlayerMovement;
    public Transform PlayerInteractTransform;
    public BreathingScript BreathingScript;
    public TutorialScript TutorialScript;
    public ShopInteractable ShopInteractable;
    public UIShopManager UIShopManager;
    public PostSignController PostSignController;

    // Game state variables
    private bool _isPlayerMovementEnabled = true; // Never access this directly, use the public property below
    public bool IsPlayerMovementEnabled {
        get => _isPlayerMovementEnabled;
        set {
            if (!value) {
                Rigidbody playerRigidBody = PlayerMovement?.GetRigidbody();
                if (playerRigidBody != null) {
                    playerRigidBody.linearVelocity = Vector3.zero;
                    playerRigidBody.angularVelocity = Vector3.zero;
                }
                this.PlayerMovement.OnMoveCanceled();
            }
            _isPlayerMovementEnabled = value;
        }
    }

    public int Money = 0;

    public bool firstEnteredOcean;

    public bool startedGame = true;
    public bool startedGameFinished;

    public bool firstTreasureCollected;
    public bool firstTreasureFinished;

    public bool firstSuitUpgrade;
    public bool firstSuitUpgradeFinished;

    public bool firstDied;
    public bool firstDiedFinished;

    public bool firstStoryCollected;
    public bool firstStoryFinished;

    public bool secondStoryCollected;
    public bool secondStoryFinished;

    public bool thirdStoryCollected;
    public bool thirdStoryFinished;

    public bool fourthStoryCollected;
    public bool fourthStoryFinished;

    public bool secretDialougeTriggered;
    public bool secretDialougeActive;
    public bool secretDialougeFinished;

    public bool endingTriggered = false;

    private void Start() {
        QualitySettings.vSyncCount = 0; // Disable VSync so targetFrameRate works
        Application.targetFrameRate = 60; // Set this to whatever FPS you want
    }

    public int GetHeldItemsValue() {
        if (PlayerInteractTransform == null) {
            Debug.LogWarning("PlayerInteractTransform is not assigned.");
            return 0;
        }


        Interactable[] heldItems = PlayerInteractTransform.GetComponentsInChildren<Interactable>();
        int totalValue = 0;
        foreach (Interactable item in heldItems) {
            totalValue += item.GetValueAmount();
        }
        return totalValue;
    }

    public void OnTestCall() {
        Debug.Log("Test call from GameManager");
    }

    public void UpMoveSpeed() {
        PlayerMovement.smallAcceleration += 3f;
        PlayerMovement.fastAcceleration += 4f;
        PlayerMovement.normalSpeed += 2f;
        PlayerMovement.fastSpeed += 2f;
        BreathingScript.upgradeValue -= 0.15f;
    }

    public void SlowDownPlayer() {
        GodMode.Instance.OnGodStarted(new UnityEngine.InputSystem.InputAction.CallbackContext()); // TODO REMOVE

        // SLOW
        PlayerMovement.smallAcceleration = 5f;
        PlayerMovement.fastAcceleration = 10f;
        PlayerMovement.normalSpeed = 3f;
        PlayerMovement.fastSpeed = 6f;
        BreathingScript.upgradeValue = 1f;
    }

    // Deletes the saved game state
    public void DeleteGameState() {
        PlayerPrefs.DeleteKey(ShopItemLookUp.PlayerShopItemsKey);
        ShopItemLookUp.Instance.ResetPlayerItems();
        GameManager.Instance.Money = 0;
    }

    public void TeleportPlayer(Vector3 newLocation) {
        Rigidbody playerRigidBody = this.PlayerMovement.GetRigidbody();

        // Directly set the player's position to the new location
        playerRigidBody.position = newLocation;

        // Reset velocities to prevent unwanted movement after teleportation
        playerRigidBody.linearVelocity = Vector3.zero;
        playerRigidBody.angularVelocity = Vector3.zero;
    }

    public void PlayerExitOcean() {
        this.PlayerMovement.isUnderWater = false;
        this.BreathingScript.DisableBreathing();
        this.BreathingScript.enabled = false;

        PlayerMovement.zoomedOut = false;

        // Fog of war
        Camera.main.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        //FogController.Instance.DisableFogEffect();

        AudioManager.Instance.underWaterSound.mute = true;
    }

    public void PlayerEnterOcean() {
        this.PlayerMovement.isUnderWater = true;
        this.BreathingScript.enabled = true; // Making sure it is enabled

        BreathingScript.oxygenAmount = BreathingScript.intendedOxygen;

        PlayerMovement.zoomedOut = true;

        // Fog of war
        Camera.main.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        //FogController.Instance.EnableFogEffect();

        AudioManager.Instance.underWaterSound.mute = false;
    }

    public void DropAllTreasure() {
        if (PlayerInteractTransform == null) {
            Debug.LogWarning("PlayerInteractTransform is not assigned.");
            return;
        }


        SunkenTreasure[] heldItems = PlayerInteractTransform.GetComponentsInChildren<SunkenTreasure>();

        foreach (SunkenTreasure item in heldItems) {
            item.DropTreasure();
            item.DetachFromPlayer();
            Debug.Log("Dropped treasure" + item);
        }
    }

    public IEnumerator PlayRespawnAnim() {
        PlayerMovement.inCutscene = true;
        PlayerMovement.ResetMomentum();
        this.PlayerMovement.PlayDeathAnim();
        yield return new WaitForSeconds(7.25f);
        DropAllTreasure();
        PlayerMovement.isUnderWater = false;
        CheckPointManager.Instance.SetCurrentCheckPoint(CheckPointManager.EnumCheckPoint.RespawnCheckpoint);
        CheckPointManager.Instance.UseCheckpoint();

        yield return new WaitForSeconds(5f);
        PlayerMovement.rb.isKinematic = true;

        this.PlayerMovement.PlayRespawnAnim();
        yield return new WaitForSeconds(11.6f);
        PlayerMovement.inCutscene = false;
        PlayerMovement.rb.isKinematic = false;
        this.PlayerMovement.ResetAnims();
    }
}
