using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenuController : MonoBehaviour {

    [SerializeField] private Button buttonResume;
    [SerializeField] private Button buttonMainMenu;

    private void OnEnable() {
        this.buttonResume.onClick.AddListener(() => OnResumeButtonClicked(this.buttonResume));
        this.buttonMainMenu.onClick.AddListener(() => OnMainMenuButtonClicked(this.buttonMainMenu));
    }

    private void OnDisable() {
        this.buttonResume.onClick.RemoveAllListeners();
        this.buttonMainMenu.onClick.RemoveAllListeners();
    }

    private void OnMainMenuButtonClicked(Button button) {
        button.interactable = false; // Prevent multiple clicks
        GameSceneManager.Instance.LoadScene(EnumScene.MainMenu, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void OnResumeButtonClicked(Button button) {
        button.interactable = false; // Prevent multiple clicks
        GameSceneManager.Instance.TogglePauseMenu();
    }
}
