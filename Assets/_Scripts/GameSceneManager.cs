using Assets.Scripts.Enums;
using Assets.Scripts.Singleton;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : Singleton<GameSceneManager> {

    /// <summary>
    /// Indicates if the pause menu is currently loaded.
    /// </summary>
    public bool isPauseMenuLoaded {
        get {
            EnumScene[] activeScenes = GetCurrentActiveScenes();
            foreach (EnumScene scene in activeScenes) {
                if (scene == EnumScene.PauseMenu)
                    return true;
            }
            return false;
        }
    }

    public void TogglePauseMenu() {
        if (isPauseMenuLoaded) {
            UnLoadPauseMenu();
        } else {
            LoadScene(EnumScene.PauseMenu, LoadSceneMode.Additive);
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
    private IEnumerator LoadSceneCoroutine(EnumScene scene, LoadSceneMode sceneMode = LoadSceneMode.Single) {
        Time.timeScale = 0f;
        int buildIndex = GetBuildIndexByName(scene);
        yield return SceneManager.LoadSceneAsync(buildIndex, sceneMode);

        if (scene != EnumScene.PauseMenu) {
            Time.timeScale = 1f;
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
        Debug.LogError($"EnumScene {sceneName} NOT found in build settings.");
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
}

