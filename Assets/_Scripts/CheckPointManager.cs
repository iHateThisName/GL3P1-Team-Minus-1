using Assets.Scripts.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : Singleton<CheckPointManager>
{

    [SerializeField] private EnumCheckPoint currentCheckPointSelected = 0;
    private Dictionary<EnumCheckPoint, CheckPoint> checkPointDictionary = new Dictionary<EnumCheckPoint, CheckPoint>();
    public HashSet<EnumCheckPoint> avaiableCheckPoints { get; private set; } = new HashSet<EnumCheckPoint>();

    public System.Action OnAvaiableCheckPointAdded;
    [SerializeField] private Transform playerTransform;

    public void RegisterCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPointDictionary.ContainsKey(checkPoint.currentCheckPoint))
        {
            checkPointDictionary.Add(checkPoint.currentCheckPoint, checkPoint);
        }
    }

    public void SetCurrentCheckPoint(CheckPoint checkPoint)
    {
        this.currentCheckPointSelected = checkPoint.currentCheckPoint;
        this.avaiableCheckPoints.Add(checkPoint.currentCheckPoint);
        this.OnAvaiableCheckPointAdded?.Invoke();
    }

    public void SetCurrentCheckPoint(int checkPointNum)
    {
        EnumCheckPoint checkPoint = (EnumCheckPoint)checkPointNum;
        this.currentCheckPointSelected = checkPoint;
        this.avaiableCheckPoints.Add(checkPoint);
        this.OnAvaiableCheckPointAdded?.Invoke();
    }

    public void SetCurrentCheckPoint(EnumCheckPoint enumCheckPoint)
    {
        this.currentCheckPointSelected = enumCheckPoint;
        this.avaiableCheckPoints.Add(enumCheckPoint);
        this.OnAvaiableCheckPointAdded?.Invoke();
    }

    [ContextMenu("Unlock All Checkpoints")]
    public void UnlockAllCheckpoints()
    {
        foreach (EnumCheckPoint cp in System.Enum.GetValues(typeof(EnumCheckPoint)))
        {
            if (cp != EnumCheckPoint.None && cp != EnumCheckPoint.Store && cp != EnumCheckPoint.RespawnCheckpoint)
            {
                this.avaiableCheckPoints.Add(cp);
            }
        }
        this.OnAvaiableCheckPointAdded?.Invoke();
    }

    [ContextMenu("Teleport Checkpoint")]
    public void UseCheckpoint() => StartCoroutine(UseCheckpointCoroutine());
    public IEnumerator UseCheckpointCoroutine()
    {
        if (!GameSceneManager.Instance.IsTransitionSceneLoaded)
        {
            yield return StartCoroutine(GameSceneManager.Instance.LoadSceneCoroutine(EnumScene.TransitionScene, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }
        // Disable Player Movement
        GameManager.Instance.IsPlayerMovementEnabled = false;

        if (currentCheckPointSelected != EnumCheckPoint.DawnCheckPoint || currentCheckPointSelected != EnumCheckPoint.RespawnCheckpoint || currentCheckPointSelected != EnumCheckPoint.FarRightCheckPoint || currentCheckPointSelected != EnumCheckPoint.Store)
        {
            GameManager.Instance.TurnOnLight.TurnOn();
        }
        else
        {
            GameManager.Instance.TurnOnLight.TurnOff();
        }

        // Wait intill completely faded out
        yield return StartCoroutine(TransitionController.Instance.FadeOutCoroutine());

        // Turn on Kinematic to avoid physics issues during teleport
        GameManager.Instance.PlayerMovement.GetRigidbody().isKinematic = true;
        GameManager.Instance.PlayerMovement.ResetAnims();

        CheckPoint cp = this.checkPointDictionary[this.currentCheckPointSelected];
        Transform tp = cp.GetTeleportLocation();
        Debug.Log($"Teleporting player to checkpoint at position: {tp.position}");

        GameManager.Instance.TeleportPlayer(new Vector3(tp.position.x, tp.position.y, tp.position.z));
        yield return new WaitForSecondsRealtime(1f);



        // Allow Player Movement and turn off Kinematic to allow normal physics interactions
        GameManager.Instance.IsPlayerMovementEnabled = true;
        GameManager.Instance.PlayerMovement.GetRigidbody().isKinematic = false;

        GameManager.Instance.PlayerExitOcean();

        // Start Fading In
        TransitionController.Instance.FadeIn();

        //AccessibleTracker.Instance.Skipped(cp.name);
    }

    public bool IsCheckPointReached(CheckPointManager.EnumCheckPoint enumCheckPoint)
    {
        return this.avaiableCheckPoints.Contains(enumCheckPoint);
    }

    public enum EnumCheckPoint : int
    {
        None = -1, Store = 0, DawnCheckPoint = 1, TwilightCheckPoint = 2, LeftMidnightCheckPoint = 3, MiddleMidnightCheckPoint = 4, RightMidnightCheckPoint = 5, FarRightCheckPoint = 6, RespawnCheckpoint = 7,
    }
}
