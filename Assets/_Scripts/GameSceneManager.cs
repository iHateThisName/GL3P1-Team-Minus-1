using Assets.Scripts.Singleton;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : PersistenSingleton<GameSceneManager> {

    /// <summary>
    /// Indicates if the pause menu is currently loaded.
    /// </summary>
    public bool IsPauseMenuLoaded {
        get {
            EnumScene[] activeScenes = GetCurrentActiveScenes();
            foreach (EnumScene scene in activeScenes) {
                if (scene == EnumScene.PauseMenu)
                    return true;
            }
            return false;
        }
    }

    public bool IsShopMenuLoaded {
        get {
            EnumScene[] activeScenes = GetCurrentActiveScenes();
            foreach (EnumScene scene in activeScenes) {
                if (scene == EnumScene.ShopMenu)
                    return true;
            }
            return false;
        }
    }

    public bool IsTransitionSceneLoaded {
        get {
            EnumScene[] activeScenes = GetCurrentActiveScenes();
            foreach (EnumScene scene in activeScenes) {
                if (scene == EnumScene.TransitionScene)
                    return true;
            }
            return false;
        }
    }

    public void TogglePauseMenu() {
        if (this.IsPauseMenuLoaded) {
            UnLoadPauseMenu();
        } else {
            LoadScene(EnumScene.PauseMenu, LoadSceneMode.Additive);
        }
    }

    public void ToggleShopMenu() {
        if (this.IsShopMenuLoaded) {
            StartCoroutine(UnLoadShopMenuCorutine());
        } else {
            GameManager.Instance.IsPlayerMovementEnabled = false;
            LoadScene(EnumScene.ShopMenu, LoadSceneMode.Additive);
        }
    }

    public void LoadScene(string sceneName) {
        if (System.Enum.TryParse(sceneName, out EnumScene sceneEnum)) {
            LoadScene(sceneEnum);
        } else {
            UnityEngine.Debug.LogError($"Scene '{sceneName}' is not a valid EnumScene value.");
        }
    }

    /// <summary>
    /// EnumScene loading call that is publicly accessible.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    /// <param name="sceneMode">The mode in which to load the scene (Single or Additive).</param>
    public void LoadScene(EnumScene scene, LoadSceneMode sceneMode = LoadSceneMode.Single) => StartCoroutine(LoadSceneCoroutine(scene, sceneMode));

    /// <summary>
    /// Handles the actual scene loading. Here we can also display loading screens etc.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    /// <param name="sceneMode">The mode in which to load the scene (Single or Additive).</param>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator LoadSceneCoroutine(EnumScene scene, LoadSceneMode sceneMode = LoadSceneMode.Single) {
        Time.timeScale = 0f;

        int buildIndex = GetBuildIndexByName(scene);
        int TransitionSceneBuildIndex = GetBuildIndexByName(EnumScene.TransitionScene);

        if (scene != EnumScene.TransitionScene && !this.IsTransitionSceneLoaded) {
            yield return SceneManager.LoadSceneAsync(TransitionSceneBuildIndex, LoadSceneMode.Additive);
        }

        // If loading GameScene or Tutorial, use fade transitions
        if (scene == EnumScene.GameScene || scene == EnumScene.Tutorial) {
            // Starts Fading out
            yield return StartCoroutine(TransitionController.Instance.FadeOutCoroutine());

            // Unload All outer Scenes
            UnLoadeAllScenesExcept(TransitionSceneBuildIndex);

            // Wait for scene to load
            yield return SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

            // Starts Fading In
            yield return StartCoroutine(TransitionController.Instance.FadeInCoroutine());
        } else {
            yield return SceneManager.LoadSceneAsync(buildIndex, sceneMode);
        }

        if (scene != EnumScene.PauseMenu) {
            // Resume time after loading non-pause menu scenes
            Time.timeScale = 1f;
        }
    }

    private void UnloadeScene(EnumScene scene) {
        SceneManager.UnloadSceneAsync(scene.ToString());
    }

    private void UnLoadeAllScenesExcept(int sceneToKeepBuildIndex) {
        System.Collections.Generic.List<int> scenesToUnload = new System.Collections.Generic.List<int>();

        for (int i = 0; i < SceneManager.sceneCount; i++) {
            int activeScene = SceneManager.GetSceneAt(i).buildIndex;
            if (sceneToKeepBuildIndex != activeScene) {
                scenesToUnload.Add(activeScene);
            }
        }

        foreach (int index in scenesToUnload) {
            SceneManager.UnloadSceneAsync(index);
        }
    }

    /// <summary>
    /// Unloads the pause menu scene and optionally unfreezes the game.
    /// </summary>
    /// <param name="unFreeze">If true, unfreezes the game after unloading.</param>
    public void UnLoadPauseMenu(bool unFreeze = true) => StartCoroutine(UnLoadPauseMenuCorutine(unFreeze));

    /// <summary>
    /// Coroutine to unload the pause menu scene and optionally unfreeze the game.
    /// </summary>
    /// <param name="unFreeze">If true, unfreezes the game after unloading.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator UnLoadPauseMenuCorutine(bool unFreeze = true) {
        yield return SceneManager.UnloadSceneAsync(EnumScene.PauseMenu.ToString());
        if (unFreeze) {
            Time.timeScale = 1f;
        }
    }

    public void UnLoadShopMenu() => StartCoroutine(UnLoadShopMenuCorutine());

    private IEnumerator UnLoadShopMenuCorutine() {
        GameManager.Instance.IsPlayerMovementEnabled = true;
        yield return SceneManager.UnloadSceneAsync(EnumScene.ShopMenu.ToString());
    }

    /// <summary>
    /// Gets the build index of a scene by its enum value.
    /// </summary>
    /// <param name="scene">The scene enum value.</param>
    /// <returns>The build index of the scene, or -1 if not found.</returns>
    private int GetBuildIndexByName(EnumScene scene) {
        string sceneName = scene.ToString();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++) {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return i;
        }
        UnityEngine.Debug.LogError($"EnumScene {sceneName} NOT found in build settings.");
        return -1;
    }

    /// <summary>
    /// Gets the currently active scenes as an array of EnumScene enums.
    /// </summary>
    /// <returns>Array of active EnumScene enums.</returns>
    public EnumScene[] GetCurrentActiveScenes() {
        int count = SceneManager.sceneCount;
        EnumScene[] scenes = new EnumScene[count];
        for (int i = 0; i < count; i++) {
            var unityScene = SceneManager.GetSceneAt(i);
            if (System.Enum.TryParse<EnumScene>(unityScene.name, out var sceneEnum)) {
                scenes[i] = sceneEnum;
            }
        }
        return scenes;
    }

    public void LaunchGame() {
        Process.Start("steam://run/2379780");
    }
}

