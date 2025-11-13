using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : Singleton<CheckPointManager> {

    [SerializeField] private EnumCheckPoint currentCheckPointSelected = 0;
    private Dictionary<EnumCheckPoint, CheckPoint> checkPointDictionary = new Dictionary<EnumCheckPoint, CheckPoint>();
    [SerializeField] private Transform playerTransform;

    public void RegisterCheckPoint(CheckPoint checkPoint) {
        if (!checkPointDictionary.ContainsKey(checkPoint.currentCheckPoint)) {
            checkPointDictionary.Add(checkPoint.currentCheckPoint, checkPoint);
        }
    }

    public void SetCurrentCheckPoint(CheckPoint checkPoint) {
        this.currentCheckPointSelected = checkPoint.currentCheckPoint;
    }

    public void SetCurrentCheckPoint(EnumCheckPoint checkPoint) {
        this.currentCheckPointSelected = checkPoint;
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

        // Turn on Kinematic to avoid physics issues during teleport
        GameManager.Instance.PlayerMovement.GetRigidbody().isKinematic = true;

        CheckPoint cp = this.checkPointDictionary[this.currentCheckPointSelected];
        Transform tp = cp.GetTeleportLocation();
        Debug.Log($"Teleporting player to checkpoint at position: {tp.position}");

        GameManager.Instance.TeleportPlayer(new Vector3(tp.position.x, tp.position.y, tp.position.z));
        yield return new WaitForSecondsRealtime(1f);

        // Allow Player Movement and turn off Kinematic to allow normal physics interactions
        GameManager.Instance.IsPlayerMovementEnabled = true;
        GameManager.Instance.PlayerMovement.GetRigidbody().isKinematic = false;

        // Start Fading In
        TransitionController.Instance.FadeIn();
    }

    public enum EnumCheckPoint : int {
        None = -1, Store = 0, DawnCheckPoint = 1,
    }
}
