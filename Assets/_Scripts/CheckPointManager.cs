using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : Singleton<CheckPointManager> {

    [SerializeField] private int currentCheckPointIndex = 0;
    [SerializeField] private List<CheckPoint> checkPoints = new List<CheckPoint>();
    [SerializeField] private Transform playerTransform;

    public void RegisterCheckPoint(CheckPoint checkPoint) {
        if (!checkPoints.Contains(checkPoint)) {
            checkPoints.Add(checkPoint);
        }
    }

    public void SetCurrentCheckPoint(CheckPoint checkPoint) {
        int index = checkPoints.IndexOf(checkPoint);
        if (index != -1) {
            currentCheckPointIndex = index;
        }
    }

    [ContextMenu("Teleport Checkpoint")]
    public void UseCheckpoint() => StartCoroutine(UseCheckpointCoroutine());
    public IEnumerator UseCheckpointCoroutine() {
        if (!GameSceneManager.Instance.IsTransitionSceneLoaded) {
            yield return StartCoroutine(GameSceneManager.Instance.LoadSceneCoroutine(EnumScene.TransitionScene, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }
        // Disable Player Movement
        GameManager.Instance.IsPlayerMovementEnabled = false;

        // Wait intill completely faded out
        yield return StartCoroutine(TransitionController.Instance.FadeOutCoroutine());
        CheckPoint cp = checkPoints[currentCheckPointIndex];
        Transform tp = cp.GetTeleportLocation();
        Debug.Log($"Teleporting player to checkpoint at position: {tp.position}");

        GameManager.Instance.TeleportPlayer(new Vector3(tp.position.x, tp.position.y, tp.position.z));
        yield return new WaitForSecondsRealtime(1f);

        // Enable Player Movement
        GameManager.Instance.IsPlayerMovementEnabled = true;
        // Start Fading In
        TransitionController.Instance.FadeIn();
    }
}
