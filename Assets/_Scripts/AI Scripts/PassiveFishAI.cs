using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveFishAI : BaseAI {

    [Header("Fish")]
    [SerializeField] private List<Transform> potentialTargets = new List<Transform>();

    private bool isPlayerClose = false;
    private State CurrentState = State.Wandering;

    private enum State { Wandering, Fleeing }
    void Start() {
        if (this.potentialTargets.Count == 0) {
            Debug.LogWarning("No potential targets assigned to PassiveFishAI.");
        }
        StartCoroutine(OldChangeTarget());
    }

    private IEnumerator ChangeTarget() {
        int i = 0;

        while (true) {
            switch (CurrentState) {
                case State.Wandering:
                    // Set current target
                    if (potentialTargets.Count > 0)
                        SetTarget(potentialTargets[i].position);

                    // Wait until target is reached
                    yield return new WaitUntil(() => this.IsReachedEndOfPath);

                    // Move to next target (wrap around)
                    i = (i + 1) % this.potentialTargets.Count;
                    break;

                case State.Fleeing:
                    // If done fleeing and player is no longer close, return to wandering
                    if (this.IsReachedEndOfPath && !this.isPlayerClose) {
                        this.CurrentState = State.Wandering;

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
                    }
                    // If still fleeing and path reached, move further away
                    else if (this.IsReachedEndOfPath && this.isPlayerClose) {
                        MoveAwayFromPlayer(GameManager.Instance.PlayerMovement.transform.position);
                    }
                    break;
            }

            // Small delay before next check (to avoid tight loop)
            yield return new WaitForSecondsRealtime(2f);
        }
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
            } else {
                // Set current target
                SetTarget(potentialTargets[i].position);

            }

            yield return new WaitForSecondsRealtime(3f);
            // Wait until the fish reaches this target
            while (!IsReachedEndOfPath) {
                yield return new WaitForSecondsRealtime(0.5f); // check every half second
            }


            // Move to next target (wrap around)
            i = (i + 1) % potentialTargets.Count;
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
        Vector3 fleeTarget = transform.position + directionAwayFromPlayer * 5f; // Move 5 units away
        SetTarget(fleeTarget);
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            this.isPlayerClose = false;
        }
    }
}
