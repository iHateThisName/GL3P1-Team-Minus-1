using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveFishAI : BaseAI {

    [Header("Fish")]
    [SerializeField] private List<Transform> potentialTargets = new List<Transform>();

    private bool isPlayerClose = false;
    private State CurrentState = State.Wandering;
    [SerializeField] private EnumFishType fishType = EnumFishType.Cod;

    private float deafualtSpeed;
    private Coroutine speedCoroutine;

    [SerializeField] private enum EnumFishType { Shark, Cod }
    private enum State { Wandering, Fleeing, Chasing }

    [SerializeField] private Transform modelTransform;
    [SerializeField] private Animator fishAnimator;

    private const string TriggerUTurn = "TurnTrigger";
    private const string StateUTurn = "Turn";

    private Coroutine OnUTurnStateExitCoroutine;
    private void Awake() {
        if (modelTransform == null) {
            modelTransform = this.transform.GetChild(0);
        }

        if (fishAnimator == null) {
            fishAnimator = modelTransform.GetComponent<Animator>();
        }
    }
    void Start() {
        if (this.potentialTargets.Count == 0) {
            Debug.LogWarning("No potential targets assigned to PassiveFishAI.");
        }

        this.deafualtSpeed = this.speed;
        StartCoroutine(ChangeTarget());

    }

    private IEnumerator ChangeTarget() {
        int i = 0;
        bool isPlayerCloseOldValue = this.isPlayerClose;

        while (true) {
            switch (CurrentState) {
                case State.Wandering:
                    this.speed = this.deafualtSpeed;
                    // Set current target
                    SetTarget(potentialTargets[i].position);
                    // Move to next target (wrap around)
                    i = (i + 1) % potentialTargets.Count;
                    break;

                case State.Fleeing:
                    StartCoroutine(TriggerSmoothSpeedChange(this.deafualtSpeed * 5f, 0.5f));

                    yield return new WaitUntil(() => this.IsReachedEndOfPath);
                    // If done fleeing and player is no longer close, return to wandering
                    if (!this.isPlayerClose) {
                        this.CurrentState = State.Wandering;

                        i = ClosestTarget();
                    }
                    // If still fleeing and path reached, move further away
                    else if (this.isPlayerClose) {
                        MoveAwayFromPlayer(GameManager.Instance.PlayerMovement.transform.position);
                    }
                    break;

                case State.Chasing:
                    if (this.isPlayerClose) {
                        IsTargetMoving = true;
                        StartCoroutine(TriggerSmoothSpeedChange(this.deafualtSpeed * 3f, 0.5f));
                    } else {
                        IsTargetMoving = false;
                        this.CurrentState = State.Wandering;
                        i = ClosestTarget();
                    }
                    break;
            }
            // Wait until target is reached or the wandering state changes
            yield return new WaitUntil(() => this.IsReachedEndOfPath || this.CurrentState != State.Wandering || this.isPlayerClose != isPlayerCloseOldValue);
            isPlayerCloseOldValue = this.isPlayerClose;
        }
    }

    private IEnumerator TriggerSmoothSpeedChange(float targetSpeed, float duration) {
        if (speedCoroutine != null) {
            StopCoroutine(this.speedCoroutine);
        }

        this.speedCoroutine = StartCoroutine(SmoothSpeedChange(targetSpeed, duration));
        yield return new WaitUntil(() => this.speed == targetSpeed);
        yield return new WaitForSecondsRealtime(1f); // Maintain target speed for a moment
        StopCoroutine(this.speedCoroutine);
        this.speedCoroutine = StartCoroutine(SmoothSpeedChange(this.deafualtSpeed, duration * 0.2f));
    }

    private IEnumerator SmoothSpeedChange(float targetSpeed, float duration) {
        float startSpeed = this.speed;
        float elapsed = 0f;
        while (elapsed < duration) {
            this.speed = Mathf.Lerp(startSpeed, targetSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        this.speed = targetSpeed;
    }

    private int ClosestTarget() {
        int i;
        // Find the closest target to return to
        float closestDistance = float.MaxValue;
        int closestIndex = 0;
        for (int j = 0; j < this.potentialTargets.Count; j++) {
            float distance = Vector3.Distance(this.transform.position, this.potentialTargets[j].position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestIndex = j;
            }
        }
        i = closestIndex;
        return i;
    }

    private void MoveAwayFromPlayer(Vector3 player) {
        Vector3 directionAwayFromPlayer = (transform.position - player).normalized;
        Vector3 fleeTarget = transform.position + directionAwayFromPlayer * 25f; // Move 10 units away
        SetTarget(fleeTarget);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (this.fishType == EnumFishType.Cod) {
                // When player enters trigger, move away from player
                MoveAwayFromPlayer(other.transform.position);
                this.isPlayerClose = true;
                this.CurrentState = State.Fleeing;
            } else if (this.fishType == EnumFishType.Shark) {
                // Sharks do not flee
                this.isPlayerClose = true;
                this.IsTargetMoving = true;
                this.CurrentState = State.Chasing;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.isPlayerClose = false;
        }
    }

    protected override void OnFlipDirection(bool facingLeft) {
        base.OnFlipDirection(facingLeft); // Call base method
        FlipModel();
    }

    public void FlipModel() {
        // Flip the model visually
        Vector3 scale = modelTransform.localScale;
        scale.y *= -1f;
        modelTransform.localScale = scale;
        this.modelTransform.transform.localScale = scale;

        Debug.Log($"Flipping model, the new localScale is : {this.modelTransform.transform.localScale}");

    }

    protected override void OnUTurn() {
        base.OnUTurn(); // Call base method
        //this.fishAnimator.SetTrigger(TriggerUTurn);
        FlipModel();

        //if (this.OnUTurnStateExitCoroutine == null) {
        //    this.OnUTurnStateExitCoroutine = StartCoroutine(OnUTurnStateExit());
        //}
    }

    private IEnumerator OnUTurnStateExit() {
        AnimatorStateInfo stateInfo = fishAnimator.GetCurrentAnimatorStateInfo(0);
        stateInfo.IsName(StateUTurn);

        // Wait until state starts
        yield return new WaitUntil(() => !stateInfo.IsName(StateUTurn));

        FlipModel();
        // Wait until state ends
        yield return new WaitUntil(() => stateInfo.IsName(StateUTurn));

        this.OnUTurnStateExitCoroutine = null;
    }
}
