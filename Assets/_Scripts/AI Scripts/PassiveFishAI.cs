using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveFishAI : BaseAI {

    [Header("Fish")]
    [SerializeField] private List<Transform> potentialTargets = new List<Transform>();

    private bool isPlayerClose = false;
    private State CurrentState = State.Wandering;

    private float deafualtSpeed;
    private Coroutine speedCoroutine;

    private enum State { Wandering, Fleeing }
    void Start() {
        if (this.potentialTargets.Count == 0) {
            Debug.LogWarning("No potential targets assigned to PassiveFishAI.");
        }

        this.deafualtSpeed = this.speed;
        StartCoroutine(ChangeTarget());
    }

    private IEnumerator ChangeTarget() {
        int i = 0;

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
            }
            // Wait until target is reached or the wandering state changes
            yield return new WaitUntil(() => this.IsReachedEndOfPath || this.CurrentState != State.Wandering);
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

    private IEnumerator OldChangeTarget() {
        int i = 0;

        while (true) {
            if (this.CurrentState == State.Fleeing && IsReachedEndOfPath && this.isPlayerClose == false) {
                this.CurrentState = State.Wandering;

                // Go to the closest target after fleeing
                float closestDistance = float.MaxValue;
                for (int j = 0; j < potentialTargets.Count; j++) {
                    float distance = Vector3.Distance(transform.position, potentialTargets[j].position);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        i = j;
                    }
                }
            }

            if (this.CurrentState == State.Fleeing && this.isPlayerClose && IsReachedEndOfPath) {
                MoveAwayFromPlayer(GameManager.Instance.PlayerMovement.transform.position);
            } else if (this.CurrentState == State.Wandering) {
                // Set current target
                SetTarget(potentialTargets[i].position);
                // Move to next target (wrap around)
                i = (i + 1) % potentialTargets.Count;
            }


            // Wait until target is reached
            yield return new WaitUntil(() => this.IsReachedEndOfPath || this.CurrentState != State.Wandering);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // When player enters trigger, move away from player
            MoveAwayFromPlayer(other.transform.position);
            this.isPlayerClose = true;
            this.CurrentState = State.Fleeing;
        }
    }

    private void MoveAwayFromPlayer(Vector3 player) {
        Vector3 directionAwayFromPlayer = (transform.position - player).normalized;
        Vector3 fleeTarget = transform.position + directionAwayFromPlayer * 10f; // Move 10 units away
        SetTarget(fleeTarget);
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.isPlayerClose = false;
        }
    }
}
