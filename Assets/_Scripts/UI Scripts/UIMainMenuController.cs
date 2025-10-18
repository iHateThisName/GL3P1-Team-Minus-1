using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour {

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    private void Awake() {
        this.newGameButton.gameObject.SetActive(false);
        this.continueButton.gameObject.SetActive(false);
    }

    private IEnumerator Start() {

        yield return new WaitUntil(() => ShopItemLookUp.Instance.IsInitialized);

        if (ShopItemLookUp.Instance.GetAllPlayerItems().Count > 0) {
            this.continueButton.gameObject.SetActive(true);
            this.newGameButton.GetComponentInChildren<TMP_Text>().text = "New Game";

            // Add listeners when there is a save file
            this.continueButton.onClick.AddListener(() => OnStartNewGamePressed(button: this.continueButton, deleteSaveFile: false));
            this.newGameButton.onClick.AddListener(() => OnStartNewGamePressed(button: this.newGameButton, deleteSaveFile: true));

        } else {
            this.continueButton.gameObject.SetActive(false);
            this.newGameButton.onClick.AddListener(() => OnStartNewGamePressed(button: this.newGameButton, deleteSaveFile: false));
        }
        this.newGameButton.gameObject.SetActive(true);

    }
    private void OnDisable() {
        this.newGameButton.onClick.RemoveAllListeners();
        this.continueButton.onClick.RemoveAllListeners();
    }

    public void OnStartNewGamePressed(Button button, bool deleteSaveFile) {
        button.interactable = false; // Prevent multiple clicks

        if (deleteSaveFile) {
            // Clear previous saved data
            GameManager.Instance.DeleteGameState();
        }
        GameSceneManager.Instance.LoadScene(EnumScene.GameScene);
    }
}
