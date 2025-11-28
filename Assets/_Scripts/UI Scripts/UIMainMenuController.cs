using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour {

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button howToPlayButton;

    private void Awake() {
        this.newGameButton.gameObject.SetActive(false);
        this.continueButton.gameObject.SetActive(false);
        this.howToPlayButton.gameObject.SetActive(false);
    }

    private IEnumerator Start() {

        yield return new WaitUntil(() => ShopItemLookUp.Instance.IsInitialized);

        if (ShopItemLookUp.Instance.GetAllPlayerItems().Count > 0 || GameManager.Instance.Money != 0) {
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
        this.howToPlayButton.gameObject.SetActive(true);

        this.howToPlayButton.onClick.AddListener(() => OnHowToPlayPressed());

#if UNITY_WEBGL
        this.howToPlayButton.interactable = false;
#endif
    }
    private void OnDisable() {
        this.newGameButton.onClick.RemoveAllListeners();
        this.continueButton.onClick.RemoveAllListeners();
        this.howToPlayButton.onClick.RemoveAllListeners();
    }

    public void OnStartNewGamePressed(Button button, bool deleteSaveFile) {
        button.interactable = false; // Prevent multiple clicks

        if (deleteSaveFile) {
            // Clear previous saved data
            GameManager.Instance.DeleteGameState();
        }
        GameSceneManager.Instance.LoadScene(EnumScene.GameScene);
    }

    public void OnHowToPlayPressed() {
        //GameSceneManager.Instance.LoadScene(EnumScene.Tutorial);
#if !UNITY_WEBGL
        Application.Quit();
#endif

    }
}
