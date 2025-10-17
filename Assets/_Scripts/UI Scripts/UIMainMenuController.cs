using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuController : MonoBehaviour {

    [SerializeField] private Button startGameButton;


    private void OnEnable() {
        this.startGameButton.onClick.AddListener(() => OnStartGamePressed(this.startGameButton));
    }
    private void OnDisable() {
        this.startGameButton.onClick.RemoveAllListeners();
    }

    public void OnStartGamePressed(Button button) {
        button.interactable = false; // Prevent multiple clicks
        GameSceneManager.Instance.LoadScene(EnumScene.GameScene);
    }
}
